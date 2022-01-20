using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Repositories
	{
		internal static ComponentRepository Components => (ComponentRepository)Repositories.GetRepository<ComponentRepository>();
	}
	internal class ComponentRepository : GameRepository
	{
		private Dictionary<System.Type, List<GameComponent>> allComponents = new Dictionary<System.Type, List<GameComponent>>();
		internal Dictionary<System.Type, GameComponentBuilder> builders = new Dictionary<System.Type, GameComponentBuilder>();
		internal Dictionary<string, GameComponentBuilder> buildersByString = new Dictionary<string, GameComponentBuilder>();
		public ComponentRepository()
		{
			repoName = "components";
			dbPath = "data/components.sqlite";
			schemaFields = new List<DatabaseField>() 
			{ 
				Field.Id,
				Field.Name,
				Field.Gender, 
				Field.Aliases,
				Field.Components,
				Field.Flags,
				Field.Location
			};
		}
		internal GameComponent? MakeAndConfigureComponent(GameComponentBuilder builder)
		{
			var newComp = (GameComponent)CreateRepositoryType(builder);
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
		internal override void Populate()
		{
			foreach(GameComponentBuilder gameCompBuild in Game.InstantiateSubclasses<GameComponentBuilder>())
			{
				if(gameCompBuild.ComponentType != null)
				{
					Game.LogError($"Built builder for {gameCompBuild.ComponentType.ToString()}");
					builders.Add(gameCompBuild.ComponentType, gameCompBuild);
					buildersByString.Add(gameCompBuild.ComponentType.ToString(), gameCompBuild);
					allComponents.Add(gameCompBuild.ComponentType, new List<GameComponent>());
				}
			}
			base.Populate();
		}
		internal IGameEntity CreateRepositoryType(GameComponentBuilder builder) 
		{
			return builder.MakeComponent();
		}

		internal override IGameEntity CreateRepositoryType(string? additionalClassInfo) 
		{
			if(additionalClassInfo != null)
			{
				var newComp = MakeComponent(additionalClassInfo);
				if(newComp != null)
				{
					return newComp;
				}
			}
			return new GameComponent();
		}
		internal override string? GetAdditionalClassInfo(Dictionary<DatabaseField, object> record)
		{
			return (string)record[Field.ComponentType];
		}
	}
}
