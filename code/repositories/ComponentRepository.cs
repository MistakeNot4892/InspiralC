using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal static partial class Repositories
	{
		internal static ComponentRepository Components { get { return (ComponentRepository)Repositories.GetRepository<ComponentRepository>(); } }
	}
	internal class ComponentRepository : GameRepository
	{
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
		internal override void PostInitialize() 
		{
		}
		internal override IGameEntity CreateRepositoryType(string? additionalClassInfo) 
		{
			if(additionalClassInfo != null && Modules.Components.buildersByString.ContainsKey(additionalClassInfo))
			{
				var newComp = Modules.Components.MakeComponent(additionalClassInfo);
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
