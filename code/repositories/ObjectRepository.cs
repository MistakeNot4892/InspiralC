using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal class ObjectRepository : GameRepository
	{
		private Dictionary<long, long> postInitLocations;
		internal ObjectRepository()
		{
			postInitLocations = new Dictionary<long, long>();
			repoName = "objects";
			dbPath = "data/objects.sqlite";
			schemaFields = new Dictionary<string, (System.Type, string)>() 
			{
				{"name",       (typeof(string), "'object'")},
				{"gender",     (typeof(string), $"'{Text.GenderInanimate}'")}, 
				{"aliases",    (typeof(string), "''")},
				{"components", (typeof(string), "''")},
				{"flags",      (typeof(int),    "-1")},
				{"location",   (typeof(int),    "0")}
			};
		}
		internal override void InstantiateFromRecord(DatabaseRecord record) 
		{
			GameObject gameObj = (GameObject)CreateRepositoryType((long)record.fields["id"]);
			gameObj.name =    record.fields["name"].ToString();
			gameObj.flags =   (long)record.fields["flags"];
			gameObj.gender =  Modules.Gender.GetByTerm(record.fields["gender"].ToString());
			gameObj.aliases = JsonConvert.DeserializeObject<List<string>>(record.fields["aliases"].ToString());

			foreach(string comp in JsonConvert.DeserializeObject<List<string>>(record.fields["components"].ToString()))
			{
				gameObj.AddComponent(Game.GetTypeFromString(comp));
			}
			records.Add(gameObj.id, gameObj);
			postInitLocations.Add(gameObj.id, (long)record.fields["location"]);
		}
		internal void LoadComponentData(GameObject gameObj)
		{
		}
		internal override GameEntity CreateRepositoryType(long id) 
		{
			return new GameEntity(id);
		}
		internal override void PostInitialize() 
		{
			foreach(KeyValuePair<long, long> loc in postInitLocations)
			{
				if(loc.Value > 0)
				{
					GameObject obj =   (GameObject)GetByID(loc.Key);
					GameObject other = (GameObject)GetByID(loc.Value);
					if(obj != null && other != null)
					{
						obj.Move(other);
					}
				}
			}
			foreach(KeyValuePair<long, GameEntity> obj in records)
			{
				LoadComponentData((GameObject)obj.Value);
			}
			foreach(KeyValuePair<long, GameEntity> obj in records)
			{
				GameObject gameObj = (GameObject)obj.Value;
				foreach(KeyValuePair<System.Type, GameComponent> comp in gameObj.components)
				{
					comp.Value.FinalizeObjectLoad();
				}
			}
		}
	}
}
