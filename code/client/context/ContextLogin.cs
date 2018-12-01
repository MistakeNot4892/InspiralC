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
			List<string> splashText = new List<string>();
			splashText.Add("- Enter a username to log in.");
			splashText.Add("- Enter <register [username]> to register a new account.");
			viewer.WriteLine(Text.FormatPopup("Welcome to Inspiral, Coalescence, Ringdown", splashText));
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
			invoker.shell = (GameObject)Game.Objects.Get(invoker.currentAccount.objectId);
			invoker.shell.AddComponent(Components.Client);
			ClientComponent clientComp = (ClientComponent)invoker.shell.GetComponent(Components.Client);
			clientComp.Login(invoker);
			Clients.LogoutDuplicateAccounts(invoker);
			invoker.SetContext(new ContextGeneral());			
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
							invoker.currentAccount = acct;
							invoker.WriteLine("Enter your password.");
							loginState.Remove(invoker);
							loginState.Add(invoker, "entering_password");
						}
						break;
					case "entering_password":
						bool correctPass = invoker.currentAccount.CheckPassword(rawCommand);
						if(correctPass)
						{
							invoker.WriteLine("Password correct.");
							HandleLogin(invoker);
						}
						else
						{
							invoker.WriteLine("Incorrect password.");
							invoker.currentAccount = null;
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
							invoker.currentAccount = Game.Accounts.CreateAccount(invoker.id, BCrypt.Net.BCrypt.HashPassword(passwordConfirmations[invoker], 10));
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