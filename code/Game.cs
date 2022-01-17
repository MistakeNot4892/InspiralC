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

		internal static AccountRepository Accounts
		{ 
			get { return s_accounts; }
			set { s_accounts = value; }
		}
		internal static ObjectRepository Objects
		{ 
			get { return s_objects; }
			set { s_objects = value; }
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
			foreach(GameClient client in Clients.Connected)
			{
				client.Farewell("The server is shutting down. Goodbye!");
			}
			Objects.Exit();
			Accounts.Exit();
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
