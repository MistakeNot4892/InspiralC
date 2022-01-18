using System.Collections.Generic;
using System.Linq;

namespace inspiral
{

	internal class GameModule
	{
		internal static List<GameModule> AllModules = new List<GameModule>();
		internal GameModule()
		{
			AllModules.Add(this);
		}
		internal virtual void Initialize() {}
		internal virtual void PostInitialize() {}
	}
	internal partial class Modules
	{
		internal void InitializeModules()
		{
			Program.Game.LogError($"Initializing modules.");
			foreach(GameModule module in GameModule.AllModules)
			{
				module.Initialize();
			}
			Program.Game.LogError($"Done.");
		}
		internal void PostInitializeModules()
		{
			Program.Game.LogError($"Post-initializing modules.");
			foreach(GameModule module in GameModule.AllModules)
			{
				module.PostInitialize();
			}
			Program.Game.LogError($"Done.");
		}
	}
}