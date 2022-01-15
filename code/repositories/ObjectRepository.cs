using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal static partial class Field
	{
		internal const string Name =       "name";
		internal const string Gender =     "gender";
		internal const string Aliases =    "aliases";
		internal const string Components = "components";
		internal const string Flags =      "flags";
		internal const string Location =   "location";
	}
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
				{Field.Name,       (typeof(string), "'object'")},
				{Field.Gender,     (typeof(string), $"'{Text.GenderInanimate}'")}, 
				{Field.Aliases,    (typeof(string), "''")},
				{Field.Components, (typeof(string), "''")},
				{Field.Flags,      (typeof(int),    "-1")},
				{Field.Location,   (typeof(int),    "0")}
			};
		}
		internal override void InstantiateFromRecord(DatabaseRecord record) 
		{
			GameObject gameObj = (GameObject)CreateRepositoryType((long)record.fields[Field.Id]);
			gameObj.name =    record.fields[Field.Name].ToString();
			gameObj.flags =   (long)record.fields[Field.Flags];
			gameObj.gender =  Modules.Gender.GetByTerm(record.fields[Field.Gender].ToString());
			gameObj.aliases = JsonConvert.DeserializeObject<List<string>>(record.fields[Field.Aliases].ToString());

			foreach(string comp in JsonConvert.DeserializeObject<List<string>>(record.fields[Field.Components].ToString()))
			{
				gameObj.AddComponent(Game.GetTypeFromString(comp));
			}
			records.Add(gameObj.GetLong(Field.Id), gameObj);
			postInitLocations.Add(gameObj.GetLong(Field.Id), (long)record.fields[Field.Location]);
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
