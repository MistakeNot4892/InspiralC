using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal static partial class Field
	{
		internal static DatabaseField Name = new DatabaseField(
			"name", "object",
			typeof(string), true, true);
		internal static DatabaseField Gender = new DatabaseField(
			"gender", Text.GenderInanimate,
			typeof(string), true, true);
		internal static DatabaseField Aliases = new DatabaseField(
			"aliases", "", 
			typeof(string), true, true);
		internal static DatabaseField Components = new DatabaseField(
			"components", "",
			typeof(Dictionary<string, long>), true, true);
		internal static DatabaseField Flags = new DatabaseField(
			"flags", -1,
			typeof(int), true, true);
		internal static DatabaseField Location = new DatabaseField(
			"location", 0,
			typeof(long), true, false);
	}
	internal class ObjectRepository : GameRepository
	{
		private Dictionary<long, long> _postInitLocations = new Dictionary<long, long>();
		internal List<string> SelfReferenceTokens = new List<string>() { "me", "self", "myself" };
		internal ObjectRepository()
		{
			repoName = "objects";
			dbPath = "data/objects.sqlite";
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
			GameObject gameObj = (GameObject)CreateRepositoryType((long)record[Field.Id]);
			gameObj.SetValue(Field.Name,    record[Field.Name].ToString());
			gameObj.SetValue(Field.Flags,   (long)record[Field.Flags]);
			gameObj.SetValue(Field.Gender,  record[Field.Gender].ToString());
			gameObj.SetValue(Field.Aliases, JsonConvert.DeserializeObject<List<string>>(record[Field.Aliases].ToString()));

			foreach(string comp in JsonConvert.DeserializeObject<List<string>>(record[Field.Components].ToString()))
			{
				gameObj.AddComponent(Game.GetTypeFromString(comp));
			}
			records.Add(gameObj.GetValue<long>(Field.Id), gameObj);
			_postInitLocations.Add(gameObj.GetValue<long>(Field.Id), (long)record[Field.Location]);
		}
		internal void LoadComponentData(GameObject gameObj)
		{
		}
		internal override GameObject CreateRepositoryType(long id) 
		{
			GameObject newObj = new GameObject();
			newObj.SetValue<long>(Field.Id, id);
			return newObj;
		}
		internal override void PostInitialize() 
		{
			foreach(KeyValuePair<long, long> loc in _postInitLocations)
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
			foreach(KeyValuePair<long, IGameEntity> obj in records)
			{
				LoadComponentData((GameObject)obj.Value);
			}
			foreach(KeyValuePair<long, IGameEntity> obj in records)
			{
				GameObject gameObj = (GameObject)obj.Value;
				foreach(KeyValuePair<System.Type, GameComponent> comp in gameObj.Components)
				{
					comp.Value.FinalizeObjectLoad();
				}
			}
		}
		internal GameObject CreateFromTemplate(string objString)
		{
			return CreateRepositoryType(0);
		}
		internal List<string> GetTemplateNames()
		{
			return new List<string>() { "none" };
		}
	}
}
