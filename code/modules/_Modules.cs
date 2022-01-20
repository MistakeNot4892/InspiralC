using System.Collections.Generic;

namespace inspiral
{

	internal class GameModule
	{
		internal virtual void Initialize()
		{ 
			Game.LogError($"Initializing module {GetType().ToString()}.");
		}
		internal virtual void PostInitialize() {}
	}
	internal static partial class Modules
	{
		internal static Dictionary<System.Type, GameModule> AllModules = new Dictionary<System.Type, GameModule>();
		internal static GameModule GetModule<T>()
		{
			return AllModules[typeof(T)];
		}
		internal static void Initialize()
		{

			Game.LogError($"Instantiating modules.");
			foreach(GameModule mod in Game.InstantiateSubclasses<GameModule>())
			{
				AllModules.Add(mod.GetType(), mod);
			}
			Game.LogError($"Initializing modules.");
			foreach(KeyValuePair<System.Type, GameModule> module in AllModules)
			{
				module.Value.Initialize();
			}
			Game.LogError($"Done.");
		}
		internal static void PostInitialize()
		{
			Game.LogError($"Post-initializing modules.");
			foreach(KeyValuePair<System.Type, GameModule> module in AllModules)
			{
				module.Value.PostInitialize();
			}
			Game.LogError($"Done.");
		}
	}
}