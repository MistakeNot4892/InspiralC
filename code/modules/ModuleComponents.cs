using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace inspiral
{
	internal static partial class Modules
	{
		internal static ComponentModule Components;
	}
	internal partial class ComponentModule : GameModule
	{
		private Dictionary<System.Type, List<GameComponent>> allComponents;
		internal Dictionary<System.Type, GameComponentBuilder> builders;
		internal override void Initialize()
		{
			Modules.Components = this;
			builders = new Dictionary<System.Type, GameComponentBuilder>();
			allComponents = new Dictionary<System.Type, List<GameComponent>>();
			Debug.WriteLine($"Initializing component builders.");
			foreach(var t in (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameComponentBuilder))
				select assemblyType))
			{
				Debug.WriteLine($"- Creating component builder {t}.");
				GameComponentBuilder builder = (GameComponentBuilder)System.Activator.CreateInstance(t);
				builders.Add(builder.ComponentType, builder);
				allComponents.Add(builder.ComponentType, new List<GameComponent>());
			}
			Debug.WriteLine($"Done.");
		}

		internal GameComponent MakeComponent<T>()
		{
			return MakeComponent(typeof(T));
		}
		internal GameComponent MakeComponent(System.Type compType)
		{
			Debug.WriteLine($"- Creating component {compType.ToString()}.");
			GameComponent returning = (GameComponent)System.Activator.CreateInstance(builders[compType].ComponentType);
			allComponents[compType].Add(returning);
			return returning;
		}
		internal List<GameComponent> GetComponents<T>()
		{
			return allComponents[typeof(T)];
		}
	}
}