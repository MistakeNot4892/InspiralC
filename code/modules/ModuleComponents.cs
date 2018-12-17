using System;
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
		private Dictionary<string, List<GameComponent>> allComponents;
		internal Dictionary<string, GameComponentBuilder> builders;

		internal override void Initialize()
		{
			Modules.Components = this;
			builders = new Dictionary<string, GameComponentBuilder>();
			allComponents = new Dictionary<string, List<GameComponent>>();
			Debug.WriteLine($"Loading components.");
			foreach(var t in (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameComponentBuilder))
				select assemblyType))
			{
				Debug.WriteLine($"- Loading component {t}.");
				GameComponentBuilder builder = (GameComponentBuilder)Activator.CreateInstance(t);
				builders.Add(builder.Name, builder);
				allComponents.Add(builder.Name, new List<GameComponent>());
			}
			Debug.WriteLine($"Done.");
		}

		internal GameComponent MakeComponent(string componentKey)
		{
			GameComponent returning = builders[componentKey].Build();
			allComponents[componentKey].Add(returning);
			return returning;
		}
		internal List<GameComponent> GetComponents(string componentKey)
		{
			return allComponents[componentKey];
		}
	}
}