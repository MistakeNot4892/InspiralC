using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace inspiral
{
	internal static partial class Components
	{
		private static Dictionary<string, List<GameComponent>> allComponents;
		internal static Dictionary<string, GameComponentBuilder> builders;

		static Components()
		{
			builders = new Dictionary<string, GameComponentBuilder>();
			allComponents = new Dictionary<string, List<GameComponent>>();
			Debug.WriteLine($"Loading components.");
			foreach(var t in (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameComponentBuilder))
				select assemblyType))
			{
				Debug.WriteLine($"Loading component {t}.");
				GameComponentBuilder builder = (GameComponentBuilder)Activator.CreateInstance(t);
				builders.Add(builder.Name, builder);
				allComponents.Add(builder.Name, new List<GameComponent>());
			}
			Debug.WriteLine($"Done.");
		}

		internal static GameComponent MakeComponent(string componentKey)
		{
			GameComponent returning = builders[componentKey].Build();
			allComponents[componentKey].Add(returning);
			return returning;
		}
		internal static List<GameComponent> GetComponents(string componentKey)
		{
			return allComponents[componentKey];
		}
	}
}