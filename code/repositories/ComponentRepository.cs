using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal partial class Repositories
	{
		internal ComponentRepository Components = new ComponentRepository();
	}
	internal class ComponentRepository : GameRepository
	{
		public ComponentRepository()
		{
			repoName = "components";
			dbPath = "data/components.sqlite";
			schemaFields = new List<DatabaseField>() 
			{ 
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
		internal override IGameEntity CreateRepositoryType() 
		{
			return new GameComponent();
		}
	}
}
