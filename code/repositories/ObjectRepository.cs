using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal partial class Repositories
	{
		internal ObjectRepository Objects = new ObjectRepository();
	}
	internal class ObjectRepository : GameRepository
	{
		internal List<string> SelfReferenceTokens = new List<string>() { "me", "self", "myself" };
		internal override bool Instantiate()
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
			return true;
		}
		internal override void Initialize() 
		{
			base.Initialize();
			foreach(KeyValuePair<IGameEntity, Dictionary<DatabaseField, object>> loadingEntity in loadingEntities)
			{
				var moveToLoc = Program.Game.Repos.Objects.GetById((long)loadingEntity.Value[Field.Location]);
				if(moveToLoc != null)
				{
					((GameObject)loadingEntity.Key).Move((GameObject)moveToLoc);
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
