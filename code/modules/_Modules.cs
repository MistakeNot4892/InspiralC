using System.Collections.Generic;
using System.Linq;

namespace inspiral
{

	internal class GameModule
	{
		internal GameModule()
		{
			Game.Modules.AllModules.Add(this);
		}
		internal virtual void Initialize() {}
		internal virtual void PostInitialize() {}
	}
	internal partial class Modules
	{
		private List<GameModule> s_modules = new List<GameModule>();
		public List<GameModule> AllModules
		{
			get { return s_modules; }
			set { s_modules = value; }
		}
		internal void InitializeModules()
		{
			Game.LogError($"Initializing modules.");
			foreach(GameModule module in AllModules)
			{
				module.Initialize();
			}
			Game.LogError($"Done.");
		}
		internal void PostInitializeModules()
		{
			Game.LogError($"Post-initializing modules.");
			foreach(GameModule module in AllModules)
			{
				module.PostInitialize();
			}
			Game.LogError($"Done.");
		}
	}
}