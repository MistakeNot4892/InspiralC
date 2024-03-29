using System.Linq;
using System.Collections.Generic;

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
				viewer.shell,
				"Inspiral, Coalescence, Ringdown", 
				$"{Colours.Fg("- Enter your character name to log in.", GlobalConfig.GetColour(Text.ColourDefaultHighlight))}\n{Colours.Fg("- Enter ",  viewer.shell.GetColour(Text.ColourDefaultHighlight))}{Colours.Fg("register [username]", viewer.shell.GetColour(Text.ColourDefaultPromptHighlight))}{Colours.Fg(" to register a new account.",  viewer.shell.GetColour(Text.ColourDefaultHighlight))}",viewer.config.wrapwidth
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
			GameObject? wakeShell = null;
			if(invoker.account != null)
			{
				var getShell = Repositories.Objects.GetById(invoker.account.GetValue<ulong>(Field.ShellId));
				if(getShell != null)
				{
					wakeShell = (GameObject)getShell;
				}
			}
			if(wakeShell == null)
			{
				wakeShell = Repositories.Objects.CreateFromTemplate(GlobalConfig.DefaultShellTemplate);
			}
			GameObject wakingShell = (GameObject)wakeShell;
			wakingShell.ShowNearby(wakingShell, $"{wakingShell.GetShortDesc()} wakes up.");
			wakingShell.SetValue<string>(Field.RoomDesc, "$Short$ is here.");

			invoker.shell = wakingShell;
			var clientComp = invoker.shell.GetComponent<ClientComponent>();
			if(clientComp != null)
			{
				ClientComponent oldClient = (ClientComponent)clientComp;
				oldClient.client?.Farewell("Another connection has been made with this account, so you are being logged out. Goodbye!");
			}
			clientComp = invoker.shell.AddComponent<ClientComponent>();
			if(clientComp != null)
			{
				ClientComponent newClient = (ClientComponent)clientComp;
				newClient.Login(invoker);
			}
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
					if(Repositories.Accounts.GetAccountByUser(newUser) != null)
					{
						invoker.WriteLine($"An account already exists with that username.");
					}
					else if(!ValidateUsername(newUser))
					{
						invoker.WriteLine($"Usernames must be 2 to 16 characters int and can only contain letters.");
					}
					else
					{
						invoker.clientId = newUser;
						loginState.Remove(invoker);
						loginState.Add(invoker, "registering_entering_password");
						invoker.WriteLine("Enter a new password of at least 6 characters, including at least one number or symbol. Remember that Telnet is not secure; do not reuse an important personal password.");
					}
				}
			}
			else
			{
				switch(loginState[invoker])
				{
					case "connected":
						var userAcct = Repositories.Accounts.GetAccountByUser(command);
						if(userAcct == null)
						{
							invoker.WriteLine($"No account exists for '{command}'. Use {Colours.Fg("register [username]",  invoker.shell.GetColour(Text.ColourDefaultHighlight))} to create one.");
						}
						else
						{
							PlayerAccount acct = (PlayerAccount)userAcct;
							invoker.account = acct;
							invoker.WriteLine("Enter your password.");
							loginState.Remove(invoker);
							loginState.Add(invoker, "entering_password");
						}
						break;
					case "entering_password":
						if(invoker.account == null)
						{
							return false; // This is weird and shouldn't happen.
						}
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
							invoker.WriteLine($"Passwords must be 6 to 30 characters int and must contain at least one symbol or number.");
						}
						else
						{
							passwordConfirmations.Remove(invoker);
							passwordConfirmations.Add(invoker, BCrypt.Net.BCrypt.HashPassword(rawCommand));
							loginState.Remove(invoker);
							loginState.Add(invoker, "registering_confirming_password");
							invoker.WriteLine("Please reenter your password to confirm.");
						}
						break;
					case "registering_confirming_password":
						if(passwordConfirmations.ContainsKey(invoker) && BCrypt.Net.BCrypt.Verify(rawCommand, passwordConfirmations[invoker]))
						{
							invoker.account = Repositories.Accounts.CreateAccount(invoker.clientId, passwordConfirmations[invoker]);
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
			invoker.SendPrompt();
			return true;
		}
	}
}