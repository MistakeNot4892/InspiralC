using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inspiral
{
	internal static class Game
	{
		public static System.Random Random = new System.Random();
		public static Dictionary<string, System.Type> TypesByString = new Dictionary<string, System.Type>();
		public static bool InitComplete = false;
		internal static void Initialize() 
		{
			// Populate modules.
			Modules.InitializeModules();
			Modules.PostInitializeModules();

			// Populate repos.
			Repositories.Populate();
			Repositories.Initialize();
			Repositories.PostInitialize();	

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
			Repositories.ExitRepos();
			Database.Exit();
		}

		internal static System.Type? GetTypeFromString(string typeString)
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
		internal static void LogError(string error)
		{
			// todo: logging framework
			System.Diagnostics.Debug.WriteLine(error);
		}
		internal static List<T> InstantiateSubclasses<T>()
		{
			List<T> subclasses = new List<T>();
			foreach(var t in (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(T))
				select assemblyType))
			{
				var newInstance = System.Activator.CreateInstance(t);
				if(newInstance != null)
				{
					subclasses.Add((T)newInstance);
				}
			}
			return subclasses;
		}
	}
}
