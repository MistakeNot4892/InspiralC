using System.Collections.Generic;
using System.Threading.Tasks;

namespace inspiral
{
	internal static class Game
	{
		internal static System.Random rand = new System.Random();
		internal static Dictionary<string, System.Type> typesByString;
		internal static bool InitComplete = false;
		internal static AccountRepository Accounts;
		internal static ObjectRepository Objects;
		static Game()
		{
			typesByString = new Dictionary<string, System.Type>();
			Accounts =      new AccountRepository();
			Objects =       new ObjectRepository();
		}
		internal static void Initialize() 
		{
			Modules.Initialize();
			Accounts.Load();
			Objects.Load();

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

		internal static System.Type GetTypeFromString(string typeString)
		{
			if(!typesByString.ContainsKey(typeString))
			{
				typesByString.Add(typeString, System.Type.GetType(typeString));
			}
			return typesByString[typeString];
		}
		internal static void LogError(string error)
		{
			// todo: errors command
			System.Diagnostics.Debug.WriteLine(error);
		}
	}
}
