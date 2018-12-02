using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// It's a repo repo!
namespace inspiral
{
	internal static class Game
	{
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

			// Create port listeners.
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
		}
	}
}
