using System.Collections.Generic;
using BCrypt.Net;
using System.Linq;

namespace inspiral
{
	class ContextLogin : GameContext
	{
		private Dictionary<GameClient, string> loginState;
		private Dictionary<GameClient, string> passwordConfirmations;

		public ContextLogin()
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
			viewer.WriteLine($"{GameColours.Fg("    =========", "cyan")}{GameColours.Fg(" Welcome to Inspiral, Coalescence, Ringdown ", "boldcyan")}{GameColours.Fg("=========", "cyan")}");
			viewer.WriteLine(GameColours.Fg("\n         - Enter a username to log in.", "white"));
			viewer.WriteLine($"{GameColours.Fg("         - Enter ","white")}{GameColours.Fg("register [username]", "boldwhite")}{GameColours.Fg(" to register a new account.", "white")}");
			viewer.WriteLine(GameColours.Fg("\n    ==============================================================", "cyan"));
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
			invoker.currentGameObject.SetString("short_description", GameText.Capitalize(invoker.currentAccount.username));
			invoker.currentGameObject.SetString("room_description", $"{invoker.currentGameObject.GetString("short_description")} is here.");
			foreach(GameClient client in Program.clients)
			{
				if(client != invoker && client.currentAccount == invoker.currentAccount)
				{
					client.WriteLine("Another connection has been made with this account, so you are being logged out. Goodbye!");
					client.Quit();
				}
			}
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
					if(AccountRepository.GetAccount(newUser) != null)
					{
						invoker.WriteLine($"An account already exists with that username.");
					}
					else if(!ValidateUsername(newUser))
					{
						invoker.WriteLine($"Usernames must be 2 to 16 characters long and can only contain letters.");
					}
					else
					{
						invoker.clientId = newUser;
						loginState.Remove(invoker);
						loginState.Add(invoker, "registering_entering_password");
						invoker.WriteLine("Enter a new password. Remember that Telnet is not secure; do not reuse an important personal password.");
					}
				}
			}
			else
			{
				switch(loginState[invoker])
				{
					case "connected":
						PlayerAccount acct = AccountRepository.GetAccount(command);
						if(acct == null)
						{
							invoker.WriteLine($"No account exists for '{command}'. Use {GameColours.Fg("register [username]", "boldwhite")} to create one.");
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
						passwordConfirmations.Remove(invoker);
						passwordConfirmations.Add(invoker, rawCommand);
						loginState.Remove(invoker);
						loginState.Add(invoker, "registering_confirming_password");
						invoker.WriteLine("Please reenter your password to confirm.");
						break;
					case "registering_confirming_password":
						if(passwordConfirmations.ContainsKey(invoker) && passwordConfirmations[invoker] == rawCommand)
						{
							invoker.currentAccount = AccountRepository.CreateAccount(invoker.clientId, BCrypt.Net.BCrypt.HashPassword(passwordConfirmations[invoker], 10));
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