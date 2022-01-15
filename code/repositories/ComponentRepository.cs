using System.Collections.Generic;

namespace inspiral
{
    internal class ComponentRepository : GameRepository
    {
		internal ComponentRepository()
		{
			repoName = "components";
			dbPath = "data/objects.sqlite";
			schemaFields = new Dictionary<string, (System.Type, string)>() 
			{
				{"userName",     (typeof(string), "''")}, 
				{"passwordHash", (typeof(string), "''")},
				{"roles",        (typeof(string), "''")},
				{"objectId",     (typeof(int),    "0")}
			};
		}
    }
}