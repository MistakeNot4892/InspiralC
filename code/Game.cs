using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inspiral
{
	internal static class Game
	{
		internal static bool InitComplete = false;
		internal static AccountRepository Accounts;
		internal static ObjectRepository Objects;
		static Game()
		{
			Accounts =  new AccountRepository();
			Objects =   new ObjectRepository();
		}
		internal static void Load()
		{
			System.IO.Directory.CreateDirectory("data");
			Accounts.Load();
			Objects.Load();
		}
		internal static void Initialize() 
		{
			List<int> ports = new List<int>() {9090, 2323};
			foreach(int port in ports)
			{
				PortListener portListener = new PortListener(port);
				Task.Run(() => portListener.Begin());
			}

			Accounts.Initialize();
			Objects.Initialize();

			Accounts.PostInitialize();
			Objects.PostInitialize();
			
			InitComplete = true;
		}
		internal static void Exit()
		{
			foreach(GameClient client in Clients.clients)
			{
				client.Farewell("The server is shutting down. Goodbye!");
			}
			Accounts.Exit();
			Objects.Exit();
		}
	}
}
