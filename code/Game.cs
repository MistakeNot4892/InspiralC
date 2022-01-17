using System.Collections.Generic;
using System.Threading.Tasks;

namespace inspiral
{
	internal static class Game
	{
		private static System.Random s_random = new System.Random();
		private static Dictionary<string, System.Type> s_typesByString = new Dictionary<string, System.Type>();
		private static bool s_initComplete = false;
		private static AccountRepository s_accounts = new AccountRepository();
		private static ObjectRepository s_objects = new ObjectRepository();
		internal static System.Random Random
		{ 
			get { return s_random; }
			set { s_random = value; }
		}

		internal static Dictionary<string, System.Type> TypesByString
		{ 
			get { return s_typesByString; }
			set { s_typesByString = value; }
		}

		internal static bool InitComplete
		{ 
			get { return s_initComplete; }
			set { s_initComplete = value; }
		}
		internal static void Initialize() 
		{
			// Populate modules.
			Modules.Initialize();

			// Populate repos.
			Repos.InstantiateRepos();
			Repos.LoadRepos();
			Repos.InitializeRepos();
			Repos.PostInitializeRepos();	

			// Start listening for client connections.
			List<int> ports = new List<int>() {9090, 2323};
			foreach(int port in ports)
			{
				PortListener portListener = new PortListener(port);
				Task.Run(() => portListener.Begin());
			}		
			InitComplete = true; // We're done!
		}
		internal static void Exit()
		{
			foreach(GameClient client in Clients.Connected)
			{
				client.Farewell("The server is shutting down. Goodbye!");
			}
			Repos.ExitRepos();
			Database.Exit();
		}

		internal static System.Type GetTypeFromString(string typeString)
		{
			if(!TypesByString.ContainsKey(typeString))
			{
				TypesByString.Add(typeString, System.Type.GetType(typeString));
			}
			return TypesByString[typeString];
		}
		internal static void LogError(string error)
		{
			// todo: errors command
			System.Diagnostics.Debug.WriteLine(error);
		}
	}
}
