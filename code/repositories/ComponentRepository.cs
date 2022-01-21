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
			schemaFields = new List<DatabaseField>() { Field.Id };
		}
		internal override string GetDatabaseTableName(IGameEntity gameEntity)
		{
			GameComponent comp = (GameComponent)gameEntity;
			GameComponentBuilder builder = buildersByString[gameEntity.GetType().ToString()];
			if(builder.tableName != null)
			{
				return $"{repoName}_{builder.tableName}";
			}
			return repoName;
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

			foreach(KeyValuePair<string, GameComponentBuilder> builder in Repositories.Components.buildersByString)
			{
				if(builder.Value.schemaFields != null && builder.Value.tableName != null)
				{
					foreach(Dictionary<DatabaseField, object> record in Database.GetAllRecords(this, $"{repoName}_{builder.Value.tableName}", builder.Value.schemaFields))
					{
						ulong? eId = (ulong)record[Field.Id];
						if(eId != null && eId > 0)
						{
							loadingEntities.Add(CreateNewInstance((ulong)eId, GetAdditionalClassInfo(record)), record);
						}
					}
				}
			}

		}
		internal List<GameComponent> GetComponents<T>()
		{
			return allComponents[typeof(T)];
		}
		internal override IGameEntity CreateRepositoryType(string? additionalClassInfo) 
		{
			if(additionalClassInfo != null)
			{
				GameComponentBuilder builder = buildersByString[additionalClassInfo];
				var newComp = builder.MakeComponent();
				if(newComp != null)
				{
					foreach(DatabaseField field in builder.schemaFields)
					{
						newComp.Fields[field] = field.fieldDefault;
					}
					newComp.Fields[Field.ComponentType] = additionalClassInfo;
					allComponents[newComp.GetType()].Add(newComp);
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
