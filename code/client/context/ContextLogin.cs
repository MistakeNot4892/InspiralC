using System.Collections.Generic;

namespace inspiral
{
	class ContextLogin : GameContext
	{
		private Dictionary<GameClient, string> loginState;

		public ContextLogin()
		{
			loginState = new Dictionary<GameClient, string>();
		}

		internal override void OnContextUnset(GameClient viewer)
		{
			loginState.Remove(viewer);
		}

		internal override void OnContextSet(GameClient viewer)
		{
			loginState.Add(viewer, "connected");
			viewer.WriteLine($"{GameColours.Fg("    =========", "cyan")}{GameColours.Fg(" Welcome to Inspiral, Coalescence, Ringdown ", "boldcyan")}{GameColours.Fg("=========", "cyan")}");
			viewer.WriteLine(GameColours.Fg("\n         - Enter a username to log in.", "white"));
			viewer.WriteLine($"{GameColours.Fg("         - Enter ","white")}{GameColours.Fg("register [username]", "boldwhite")}{GameColours.Fg(" to register a new account.", "white")}");
			viewer.WriteLine(GameColours.Fg("\n    ==============================================================", "cyan"));
		}

		internal override bool TakeInput(GameClient invoker, string command, string arguments)
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
					else
					{
						AccountRepository.CreateAccount(newUser);
						invoker.WriteLine("Account created.");
						invoker.clientId = GameText.Capitalize(newUser);
						invoker.currentGameObject.SetString("short_description", invoker.clientId);
						invoker.currentGameObject.SetString("room_description", $"{invoker.currentGameObject.GetString("short_description")} is here.");
						invoker.SetContext(new ContextGeneral());
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
							loginState.Remove(invoker);
							loginState.Add(invoker, "entering_password");
							invoker.WriteLine("Enter your password.");
							loginState.Remove(invoker);
							loginState.Add(invoker, "entering_password");
						}
						break;
					case "entering_password":
						bool correctPass = invoker.currentAccount.CheckPassword(command);
						if(correctPass)
						{
							invoker.WriteLine("Password correct.");
							invoker.currentGameObject.SetString("short_description", GameText.Capitalize(invoker.currentAccount.username));
							invoker.currentGameObject.SetString("room_description", $"{invoker.currentGameObject.GetString("short_description")} is here.");
							
							bool foundPriorClient = false;
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
						else
						{
							invoker.WriteLine("Incorrect password.");
						}
						break;
					case "registering_entering_password":
						break;
					case "registering_confirming_password":
						break;
				}
			}
			return true;
		}
	}
}