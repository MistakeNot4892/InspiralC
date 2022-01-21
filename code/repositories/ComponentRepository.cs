using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Repositories
	{
		internal static ComponentRepository Components => (ComponentRepository)Repositories.GetRepository<ComponentRepository>();
	}
	internal class ComponentRepository : GameRepository
	{
		private Dictionary<string, List<GameComponent>> AllComponents = new Dictionary<string, List<GameComponent>>();
		internal Dictionary<string, GameComponentBuilder> AllBuilders = new Dictionary<string, GameComponentBuilder>();
		public ComponentRepository()
		{
			repoName = "components";
			schemaFields = new List<DatabaseField>() { Field.Id };
		}
		internal override string GetDatabaseTableName(IGameEntity gameEntity)
		{
			GameComponent comp = (GameComponent)gameEntity;
			string? compId = gameEntity.GetValue<string>(Field.ComponentId);
			if(compId != null)
			{
				GameComponentBuilder builder = AllBuilders[compId];
				if(builder.tableName != null)
				{
					return $"{repoName}_{builder.tableName}";
				}
			}
			Game.LogError($"Invalid builder trying to write to a table: {gameEntity.GetType().ToString()}");
			return repoName;
		}

		internal override void Populate()
		{

			foreach(GameComponentBuilder gameCompBuild in Game.InstantiateSubclasses<GameComponentBuilder>())
			{
				if(gameCompBuild.ComponentId != null)
				{
					Game.LogError($"Built builder for {gameCompBuild.ComponentId}");
					AllBuilders.Add(gameCompBuild.ComponentId, gameCompBuild);
					AllComponents.Add(gameCompBuild.ComponentId, new List<GameComponent>());
				}
			}

			foreach(KeyValuePair<string, GameComponentBuilder> builder in Repositories.Components.AllBuilders)
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
		internal List<GameComponent> GetComponents(string componentId)
		{
			if(AllComponents.ContainsKey(componentId))
			{
				return AllComponents[componentId];
			}
			return new List<GameComponent>();
		}
		internal override IGameEntity CreateRepositoryType(string? additionalClassInfo) 
		{
			if(additionalClassInfo != null)
			{
				GameComponentBuilder builder = AllBuilders[additionalClassInfo];
				var newComp = builder.MakeComponent();
				if(newComp != null)
				{
					foreach(DatabaseField field in builder.schemaFields)
					{
						newComp.Fields[field] = field.fieldDefault;
					}
					newComp.Fields[Field.ComponentId] = additionalClassInfo;
					AllComponents[additionalClassInfo].Add(newComp);
					return newComp;
				}
			}
			return new GameComponent();
		}
		internal override string? GetAdditionalClassInfo(Dictionary<DatabaseField, object> record)
		{
			return (string)record[Field.ComponentId];
		}
	}
}
