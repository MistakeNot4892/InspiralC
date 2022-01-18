using System.Collections.Generic;
using System.Linq;

namespace inspiral
{
	internal partial class Modules
	{
		internal ComponentModule Components = new ComponentModule();
	}
	internal partial class ComponentModule : GameModule
	{
		private Dictionary<System.Type, List<GameComponent>> allComponents = new Dictionary<System.Type, List<GameComponent>>();
		internal Dictionary<System.Type, GameComponentBuilder> builders = new Dictionary<System.Type, GameComponentBuilder>();
		internal override void Initialize()
		{
			Game.LogError($"Initializing component builders.");
			foreach(var t in (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameComponentBuilder))
				select assemblyType))
			{
				Game.LogError($"- Creating component builder {t}.");
				var builder = System.Activator.CreateInstance(t);
				if(builder != null)
				{
					GameComponentBuilder gameCompBuild = (GameComponentBuilder)builder;
					if(gameCompBuild.ComponentType != null)
					{
						builders.Add(gameCompBuild.ComponentType, gameCompBuild);
						allComponents.Add(gameCompBuild.ComponentType, new List<GameComponent>());
					}
				}
			}
			Game.LogError($"Done.");
		}

		internal GameComponent? MakeComponent<T>()
		{
			return MakeComponent(typeof(T));
		}
		internal GameComponent? MakeComponent(System.Type compType)
		{
			GameComponentBuilder builder = builders[compType];
			if(builder.ComponentType != null)
			{
				var returning = System.Activator.CreateInstance(builder.ComponentType);
				if(returning != null)
				{
					GameComponent gameCompMade = (GameComponent)returning;
					allComponents[compType].Add(gameCompMade);
					return gameCompMade;
				}
			}
			return null;
		}
		internal List<GameComponent> GetComponents<T>()
		{
			return allComponents[typeof(T)];
		}
	}
}