using System.Collections.Generic;
using System.Linq;

namespace inspiral
{
	internal static partial class Modules
	{
		internal static ComponentModule Components { get { return (ComponentModule)GetModule<ComponentModule>(); } }
	}
	internal partial class ComponentModule : GameModule
	{
		private Dictionary<System.Type, List<GameComponent>> allComponents = new Dictionary<System.Type, List<GameComponent>>();
		internal Dictionary<System.Type, GameComponentBuilder> builders = new Dictionary<System.Type, GameComponentBuilder>();
		internal Dictionary<string, GameComponentBuilder> buildersByString = new Dictionary<string, GameComponentBuilder>();
		internal override void Initialize()
		{
			Game.LogError($"Initializing component builders.");
			foreach(GameComponentBuilder gameCompBuild in Game.InstantiateSubclasses<GameComponentBuilder>())
			{
				if(gameCompBuild.ComponentType != null)
				{
					builders.Add(gameCompBuild.ComponentType, gameCompBuild);
					buildersByString.Add(gameCompBuild.ComponentType.ToString(), gameCompBuild);
					allComponents.Add(gameCompBuild.ComponentType, new List<GameComponent>());
				}
			}
			Game.LogError($"Done.");
		}
		internal GameComponent? MakeAndConfigureComponent(GameComponentBuilder builder)
		{
			GameComponent? newComp = builder.MakeComponent();
			if(newComp != null)
			{
				foreach(DatabaseField field in builder.schemaFields)
				{
					newComp.Fields[field] = field.fieldDefault;
				}
				newComp.Fields[Field.ComponentType] = newComp.GetType().ToString();
				allComponents[newComp.GetType()].Add(newComp);
			}
			return newComp;
		}
		internal GameComponent? MakeComponent(string compType)
		{
			return MakeAndConfigureComponent(buildersByString[compType]);
		}
		internal GameComponent? MakeComponent(System.Type compType)
		{
			return MakeAndConfigureComponent(builders[compType]);
		}
		internal GameComponent? MakeComponent<T>()
		{
			return MakeAndConfigureComponent(builders[typeof(T)]);
		}
		internal List<GameComponent> GetComponents<T>()
		{
			return allComponents[typeof(T)];
		}
	}
}