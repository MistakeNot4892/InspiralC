using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal static partial class Repositories
	{
		internal static ObjectRepository Objects => (ObjectRepository)Repositories.GetRepository<ObjectRepository>();
	}
	internal class ObjectRepository : GameRepository
	{
		internal List<string> SelfReferenceTokens = new List<string>() { "me", "self", "myself" };
		public ObjectRepository()
		{
			repoName = "objects";
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
		internal override void Initialize() 
		{
			base.Initialize();
			foreach(KeyValuePair<IGameEntity, Dictionary<DatabaseField, object>> loadingEntity in loadingEntities)
			{
				var moveToLoc = Repositories.Objects.GetById((ulong)loadingEntity.Value[Field.Location]);
				if(moveToLoc != null)
				{
					((GameObject)loadingEntity.Key).Move((GameObject)moveToLoc);
				}
			}
		}
		// TODO: add templating engine.
		internal GameObject CreateFromTemplate(string objString)
		{
			var newObj = (GameObject)CreateNewInstance();
			newObj.AddComponent<VisibleComponent>();
			switch(objString)
			{
				case GlobalConfig.DefaultShellTemplate:
					newObj.SetValue<string>(Field.Name, "creature");
					newObj.aliases = new List<string>() { "creature" };
					newObj.SetValue<string, VisibleComponent>(Field.ShortDesc,    "a generic creature");
					newObj.SetValue<string, VisibleComponent>(Field.ExaminedDesc, "and is a very generic creature, instantiated from a template.");
					newObj.SetValue<string, VisibleComponent>(Field.RoomDesc,     "$Short$ is here, being generic.");

					newObj.AddComponent<MobileComponent>();
					//"inspiral.MobileComponent" : { "mobtype" :      "humanoid"
					newObj.AddComponent<InventoryComponent>();
					newObj.AddComponent<BalanceComponent>();

					newObj.AddComponent<PhysicsComponent>();
					newObj.SetValue<int, PhysicsComponent>(Field.Length, 15);
					newObj.SetValue<int, PhysicsComponent>(Field.Width, 35);
					newObj.SetValue<int, PhysicsComponent>(Field.Height, 160);
					newObj.SetValue<double, PhysicsComponent>(Field.Density, 0.7);
					break;
				case "room":
					Game.LogError("Creating a room oh shit.");
					newObj.SetValue<string>(Field.Name, "room");
					newObj.aliases = new List<string>() { "room" };
					newObj.AddComponent<RoomComponent>();
					newObj.SetValue<string, VisibleComponent>(Field.ShortDesc,    "an empty room");
					newObj.SetValue<string, VisibleComponent>(Field.ExaminedDesc, "There is nothing here except silence.");
					newObj.SetValue<string, VisibleComponent>(Field.RoomDesc,     "If you can see this, please tell a dev (roomDesc placeholder for room template).");
					break;
				default:
					newObj.SetValue<string>(Field.Name, "obj");
					newObj.aliases = new List<string>() { "obj" };
					newObj.SetValue<string, VisibleComponent>(Field.ShortDesc,    "a generic object");
					newObj.SetValue<string, VisibleComponent>(Field.ExaminedDesc, "This is a very generic object, instantiated from a template.");
					newObj.SetValue<string, VisibleComponent>(Field.RoomDesc,     "$Short$ is here, being generic.");

					newObj.AddComponent<PhysicsComponent>();
					newObj.SetValue<int, PhysicsComponent>(Field.Length, 20);
					newObj.SetValue<int, PhysicsComponent>(Field.Width, 20);
					newObj.SetValue<int, PhysicsComponent>(Field.Height, 20);
					break;
			}
			Database.CreateRecord(this, newObj);
			return newObj;
		}
		internal List<string> GetTemplateNames()
		{
			return new List<string>() { "none" };
		}
	}
}
