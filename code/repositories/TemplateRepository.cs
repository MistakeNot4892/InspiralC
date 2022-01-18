using System.Collections.Generic;

namespace inspiral
{
	internal partial class Repositories
	{
		internal TemplateRepository Templates = new TemplateRepository();
	}
	internal class TemplateRepository : GameRepository
	{
		internal override bool Instantiate()
		{
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
			return true;
		}
	}
}
