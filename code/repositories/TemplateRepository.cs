using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal partial class Repos
	{
		internal static TemplateRepository Templates = new TemplateRepository();
	}
	internal class TemplateRepository : GameRepository
	{
		internal override void Instantiate()
		{
			Repos.Templates = this;
			repoName = "templates";
			dbPath = "data/templates.sqlite";
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
		internal override void InstantiateFromRecord(Dictionary<DatabaseField, object> record) 
		{
		}
		internal void LoadComponentData(GameObject gameObj)
		{
		}
		internal override GameObject? CreateRepositoryType(long id) 
		{
			return null;
		}
		internal override void PostInitialize() 
		{
		}
	}
}
