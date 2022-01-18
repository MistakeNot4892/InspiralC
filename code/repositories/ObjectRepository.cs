using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal partial class Repos
	{
		internal ObjectRepository Objects = new ObjectRepository();
	}

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
			typeof(List<string>), true, true);
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
		internal override void Instantiate()
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
			var newObj = CreateRepositoryType((long)record[Field.Id]);
			if(newObj != null)
			{
				GameObject gameObj = (GameObject)newObj;
				gameObj.SetValue(Field.Name,    record[Field.Name].ToString());
				gameObj.SetValue(Field.Flags,   (long)record[Field.Flags]);
				gameObj.SetValue(Field.Gender,  record[Field.Gender].ToString());
				gameObj.SetValue(Field.Aliases, JsonConvert.DeserializeObject<List<string>>((string)record[Field.Aliases]));

				List<string>? compList = JsonConvert.DeserializeObject<List<string>>((string)record[Field.Components]);
				if(compList != null)
				{
					foreach(string comp in compList)
					{
						System.Type? compType = Game.GetTypeFromString(comp);
						if(compType != null)
						{
							gameObj.AddComponent(compType);
						}
					}
				}
				records.Add(gameObj.GetValue<long>(Field.Id), gameObj);
				_postInitLocations.Add(gameObj.GetValue<long>(Field.Id), (long)record[Field.Location]);
			}
		}
		internal void LoadComponentData(GameObject gameObj)
		{
		}
		internal override GameObject? CreateRepositoryType(long id) 
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
					var obj =   GetById(loc.Key);
					var other = GetById(loc.Value);
					if(obj != null && other != null)
					{
						((GameObject)obj).Move((GameObject)other);
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
		// TODO: add templating engine.
		internal GameObject CreateFromTemplate(string objString)
		{
			GameObject newObj = new GameObject();
			newObj.SetValue<long>(Field.Id, GetUnusedIndex());
			newObj.AddComponent<VisibleComponent>();
			switch(objString)
			{
				case "mob":
					newObj.SetValue<string>(Field.Name, "mob");
					newObj.SetValue<List<string>>(Field.Aliases, new List<string>() { "mob" });
					newObj.SetValue<string>(Field.ShortDesc,    "a generic creature");
					newObj.SetValue<string>(Field.ExaminedDesc, "and is a very generic creature, instantiated from a template.");
					newObj.SetValue<string>(Field.RoomDesc,     "$Short$ is here, being generic.");

					newObj.AddComponent<MobileComponent>();
					//"inspiral.MobileComponent" : { "mobtype" :      "humanoid"
					newObj.AddComponent<InventoryComponent>();
					newObj.AddComponent<BalanceComponent>();
					newObj.AddComponent<PhysicsComponent>();
					newObj.SetValue<int>(Field.Length, 15);
					newObj.SetValue<int>(Field.Width, 35);
					newObj.SetValue<int>(Field.Height, 160);
					newObj.SetValue<double>(Field.Density, 0.7);
					break;
				case "room":
					newObj.SetValue<string>(Field.Name, "room");
					newObj.SetValue<List<string>>(Field.Aliases, new List<string>() { "room" });
					newObj.AddComponent<RoomComponent>();
					newObj.SetValue<string>(Field.ShortDesc,    "an empty room");
					newObj.SetValue<string>(Field.ExaminedDesc, "There is nothing here except silence.");
					newObj.SetValue<string>(Field.RoomDesc,     "If you can see this, please tell a dev (roomDesc placeholder for room template).");
					break;
				default:
					newObj.SetValue<string>(Field.Name, "object");
					newObj.SetValue<List<string>>(Field.Aliases, new List<string>() { "object" });
					newObj.SetValue<string>(Field.ShortDesc,    "a generic object");
					newObj.SetValue<string>(Field.ExaminedDesc, "This is a very generic object, instantiated from a template.");
					newObj.SetValue<string>(Field.RoomDesc,     "$Short$ is here, being generic.");
					newObj.AddComponent<PhysicsComponent>();
					newObj.SetValue<int>(Field.Length, 20);
					newObj.SetValue<int>(Field.Width, 20);
					newObj.SetValue<int>(Field.Height, 20);
					break;
			}
			return newObj;
		}
		internal List<string> GetTemplateNames()
		{
			return new List<string>() { "none" };
		}
	}
}
