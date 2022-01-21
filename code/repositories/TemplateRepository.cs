using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Repositories
	{
		internal static TemplateRepository Templates { get { return (TemplateRepository)Repositories.GetRepository<TemplateRepository>(); } }
	}
	internal class TemplateRepository : GameRepository
	{
		public TemplateRepository()
		{
			repoName = "templates";
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
	}
}
