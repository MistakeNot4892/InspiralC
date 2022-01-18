using System.Collections.Generic;
using System.Threading.Tasks;

namespace inspiral
{
	internal class Game
	{
		internal Repositories Repos = new Repositories();
		internal Modules Mods = new Modules();
		private System.Random s_random = new System.Random();
		private Dictionary<string, System.Type> s_typesByString = new Dictionary<string, System.Type>();
		private bool s_initComplete = false;
		private AccountRepository s_accounts = new AccountRepository();
		private ObjectRepository s_objects = new ObjectRepository();
		internal System.Random Random
		{ 
			get { return s_random; }
			set { s_random = value; }
		}

		internal Dictionary<string, System.Type> TypesByString
		{ 
			get { return s_typesByString; }
			set { s_typesByString = value; }
		}

		internal bool InitComplete
		{ 
			get { return s_initComplete; }
			set { s_initComplete = value; }
		}
		internal void Initialize() 
		{
			// Populate modules.
			Mods.InitializeModules();
			Mods.PostInitializeModules();

			// Populate repos.
			Repos.Populate();
			Repos.Initialize();
			Repos.PostInitialize();	

			// Start listening for client connections.
			List<int> ports = new List<int>() {9090, 2323};
			foreach(int port in ports)
			{
				PortListener portListener = new PortListener(port);
				Task.Run(() => portListener.Begin());
			}		
			InitComplete = true; // We're done!
		}
		internal void Exit()
		{
			foreach(GameClient client in Clients.Connected)
			{
				client.Farewell("The server is shutting down. Goodbye!");
			}
			Repos.ExitRepos();
			Database.Exit();
		}

		internal System.Type? GetTypeFromString(string typeString)
		{
			if(!TypesByString.ContainsKey(typeString))
			{
				System.Type? typeFromString = System.Type.GetType(typeString);
				if(typeFromString != null)
				{
					TypesByString.Add(typeString, typeFromString);
				}
			}
			return TypesByString[typeString];
		}
		internal void LogError(string error)
		{
			// todo: errors command
			System.Diagnostics.Debug.WriteLine(error);
		}
	}
}
