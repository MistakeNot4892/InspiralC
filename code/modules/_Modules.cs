using System.Collections.Generic;
using System.Linq;

namespace inspiral
{

	internal class GameModule
	{
		internal virtual void Initialize() {}
		internal virtual void PostInitialize() {}
	}
	internal static partial class Modules
	{
		private static List<GameModule> modules = new List<GameModule>();
		internal static void Initialize()
		{
			Game.LogError($"Initializing modules.");
			foreach(var t in (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameModule))
				select assemblyType))
			{
				Game.LogError($"Initializing module {t}.");
				GameModule module = (GameModule)System.Activator.CreateInstance(t);
				module.Initialize();
				modules.Add(module);
			}
			PostInitialize();
		}

		internal static void PostInitialize()
		{
			Game.LogError($"Post-initializing modules.");
			foreach(GameModule module in modules)
			{
				module.PostInitialize();
			}
			Game.LogError($"Done.");
		}
	}
}