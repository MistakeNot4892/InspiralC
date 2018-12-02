using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using BCrypt.Net;

namespace inspiral
{
	class ContextLogin : GameContext
	{
		private Dictionary<GameClient, string> loginState;
		private Dictionary<GameClient, string> passwordConfirmations;
		internal ContextLogin()
		{
			loginState = new Dictionary<GameClient, string>();
			passwordConfirmations = new Dictionary<GameClient, string>();
		}
		internal override void OnContextUnset(GameClient viewer)
		{
			loginState.Remove(viewer);
			passwordConfirmations.Remove(viewer);
		}
		internal override void OnContextSet(GameClient viewer)
		{
			loginState.Add(viewer, "connected");
			passwordConfirmations.Remove(viewer);
			ShowSplashScreen(viewer);
		}
		private void ShowSplashScreen(GameClient viewer)
		{
			viewer.WriteLine(Text.FormatPopup(
				"Inspiral, Coalescence, Ringdown", 
				$"{Colours.Fg("- Enter your character name to log in.", Colours.BoldWhite)}\n{Colours.Fg("- Enter ", Colours.BoldWhite)}{Colours.Fg("register [username]", Colours.BoldYellow)}{Colours.Fg(" to register a new account.", Colours.BoldWhite)}",viewer.config.wrapwidth
				));
		}
		private bool ValidatePassword(string givenPass)
		{
			return (givenPass.Length <= 30 && 
				givenPass.Length >= 6 && 
				!givenPass.All(char.IsLetter)
				);
		}
		private bool ValidateUsername(string givenName)
		{
			return (givenName.Length <= 16 && 
				givenName.Length >= 2 && 
				givenName.All(char.IsLetter)
				);
		}
		private void HandleLogin(GameClient invoker)
		{
			invoker.shell = (GameObject)Game.Objects.Get(invoker.account.objectId);
			if(invoker.shell.HasComponent(Components.Client))
			{
				ClientComponent oldClient = (ClientComponent)invoker.shell.GetComponent(Components.Client);
				oldClient.client?.Farewell("Another connection has been made with this account, so you are being logged out. Goodbye!");
			}
			invoker.shell.AddComponent(Components.Client);
			ClientComponent clientComp = (ClientComponent)invoker.shell.GetComponent(Components.Client);
			clientComp.Login(invoker);
			invoker.SetContext(Contexts.General);
		}
		internal override bool TakeInput(GameClient invoker, string command, string rawCommand, string arguments)
		{
			if(command == "register")
			{
				string[] tokens = arguments.Split(" ");
				if(tokens.Length < 1)
				{
					invoker.WriteLine($"Please supply a desired username when registering.");
				}
				else
				{
					string newUser = tokens[0].ToLower();
					if(Game.Accounts.GetAccountByUser(newUser) != null)
					{
						invoker.WriteLine($"An account already exists with that username.");
					}
					else if(!ValidateUsername(newUser))
					{
						invoker.WriteLine($"Usernames must be 2 to 16 characters long and can only contain letters.");
					}
					else
					{
						invoker.id = newUser;
						loginState.Remove(invoker);
						loginState.Add(invoker, "registering_entering_password");
						invoker.WriteLine("Enter a new password of at least 6 characters, including at least one number or symbol.\nRemember that Telnet is not secure; do not reuse an important personal password.");
					}
				}
			}
			else
			{
				switch(loginState[invoker])
				{
					case "connected":
						PlayerAccount acct = Game.Accounts.GetAccountByUser(command);
						if(acct == null)
						{
							invoker.WriteLine($"No account exists for '{command}'. Use {Colours.Fg("register [username]", Colours.BoldWhite)} to create one.");
						}
						else
						{
							invoker.account = acct;
							invoker.WriteLine("Enter your password.");
							loginState.Remove(invoker);
							loginState.Add(invoker, "entering_password");
						}
						break;
					case "entering_password":
						bool correctPass = invoker.account.CheckPassword(rawCommand);
						if(correctPass)
						{
							invoker.WriteLine("Password correct.");
							HandleLogin(invoker);
						}
						else
						{
							invoker.WriteLine("Incorrect password.");
							invoker.account = null;
							loginState.Remove(invoker);
							loginState.Add(invoker, "connected");
							ShowSplashScreen(invoker);
						}
						break;
					case "registering_entering_password":

						if(!ValidatePassword(rawCommand))
						{
							invoker.WriteLine($"Passwords must be 6 to 30 characters long and must contain at least one symbol or number.");
						}
						else
						{
							passwordConfirmations.Remove(invoker);
							passwordConfirmations.Add(invoker, rawCommand);
							loginState.Remove(invoker);
							loginState.Add(invoker, "registering_confirming_password");
							invoker.WriteLine("Please reenter your password to confirm.");
						}
						break;
					case "registering_confirming_password":
						if(passwordConfirmations.ContainsKey(invoker) && passwordConfirmations[invoker] == rawCommand)
						{
							invoker.account = Game.Accounts.CreateAccount(invoker.id, BCrypt.Net.BCrypt.HashPassword(passwordConfirmations[invoker], 10));
							invoker.WriteLine("Account created.");
							HandleLogin(invoker);
						}
						else
						{
							invoker.WriteLine("Passwords do not match.");
							passwordConfirmations.Remove(invoker);
							loginState.Remove(invoker);
							loginState.Add(invoker, "connected");
							ShowSplashScreen(invoker);
						}
						break;
				}
			}
			return true;
		}
	}
}