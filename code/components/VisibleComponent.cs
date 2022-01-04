using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Visibles => GetComponents<VisibleComponent>();
	}

	internal static partial class Text
	{
		internal const string FieldShortDesc    = "short";
		internal const string FieldRoomDesc     =  "room";
		internal const string FieldExaminedDesc = "examined";
	}

	internal class VisibleBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(VisibleComponent);
		}
		internal override List<string> editableFields { get; set; } = new List<string>() {Text.FieldShortDesc, Text.FieldRoomDesc, Text.FieldExaminedDesc};
		internal override List<string> viewableFields { get; set; } = new List<string>() {Text.FieldShortDesc, Text.FieldRoomDesc, Text.FieldExaminedDesc};
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_visible WHERE id = @p0;";

		internal override string TableSchema  { get; set; } = $@"CREATE TABLE IF NOT EXISTS components_visible (
			id INTEGER NOT NULL PRIMARY KEY UNIQUE,
			{Text.FieldShortDesc} TEXT DEFAULT '',
			{Text.FieldRoomDesc} TEXT DEFAULT '',
			{Text.FieldExaminedDesc} TEXT DEFAULT ''
			);";
		internal override string UpdateSchema { get; set; } = $@"UPDATE components_visible SET 
			{Text.FieldShortDesc} = @p1, 
			{Text.FieldRoomDesc} = @p2, 
			{Text.FieldExaminedDesc} = @p3 
			WHERE id = @p0;";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_visible (
			id, 
			{Text.FieldShortDesc}, 
			{Text.FieldRoomDesc}, 
			{Text.FieldExaminedDesc}
			) VALUES (
			@p0, 
			@p1, 
			@p2, 
			@p3 
			);";
	}
	internal class VisibleComponent : GameComponent
	{
		internal string shortDescription = "a generic object";
		internal string roomDescription = "A generic object is here.";
		internal string examinedDescription = "This is a generic object. Fascinating stuff.";
		internal override bool SetValue(string field, string newValue)
		{
			bool success = false;
			switch(field)
			{
				case Text.FieldShortDesc:
					if(shortDescription != newValue)
					{
						shortDescription = newValue;
						success = true;
					}
					break;
				case Text.FieldRoomDesc:
					if(roomDescription != newValue)
					{
						roomDescription = newValue;
						success = true;
					}
					break;
				case Text.FieldExaminedDesc:
					if(examinedDescription != newValue)
					{
						examinedDescription = newValue;
						success = true;
					}
					break;
			}
			return success;
		}
		internal override string GetString(string field)
		{
			switch(field)
			{
				case Text.FieldShortDesc:
					return shortDescription;
				case Text.FieldRoomDesc:
					return roomDescription;
				case Text.FieldExaminedDesc:
					return examinedDescription;
				default:
					return null;
			}
		}
		internal void ExaminedBy(GameObject viewer, bool fromInside)
		{
			string mainDesc = $"{Colours.Fg(Text.Capitalize(shortDescription),Colours.BoldWhite)}.";
			if(parent.HasComponent<MobileComponent>())
			{
				string startingToken;
				string theyAre;
				string their;
				if(parent == viewer)
				{
					startingToken = "You're";
					theyAre = "You're";
					their = "Your";
				}
				else
				{
					startingToken = "That's";
					theyAre = $"{Text.Capitalize(parent.gender.They)} {parent.gender.Is}";
					their = Text.Capitalize(parent.gender.Their);
				}

				MobileComponent mob = (MobileComponent)parent.GetComponent<MobileComponent>();
				mainDesc = $"{startingToken} {mainDesc}\n{theyAre} a {mob.race}";
				if(viewer == parent)
				{
					mainDesc += $". When people look at you, they see:\n{Text.Capitalize(parent.gender.They)} {parent.gender.Is} a {mob.race}";
				}
				if(examinedDescription == null || examinedDescription.Length <= 0)
				{
					mainDesc += ".";
				}
				else if(examinedDescription[0] == '.' || examinedDescription[0] == '!' || examinedDescription[0] == '?')
				{
					mainDesc += examinedDescription;
				}
				else
				{
					mainDesc += $" {examinedDescription}";
				}

				List<string> clothing = parent.GetVisibleContents(viewer, false);
				if(clothing.Count > 0)
				{
					mainDesc += $"\n{theyAre} carrying:";
					foreach(string line in clothing)
					{
						mainDesc += $"\n{Text.Capitalize(line)}";
					}
				}
				else
				{
					mainDesc += $"\n{theyAre} completely naked.";
				}
				
				foreach(KeyValuePair<string, GameObject> bp in mob.limbs)
				{
					if(bp.Value == null)
					{
						mainDesc += $"\n{their} {bp.Key} is missing!";
					}
					else
					{
						mainDesc += $"\n{their} {bp.Key} is healthy.";
					}
				}
			}
			else
			{
				mainDesc += $"\n{Colours.Fg(examinedDescription, Colours.BoldBlack)}";
				if(parent.contents.Count > 0)
				{
					List<string> roomAppearances = parent.GetVisibleContents(viewer, false);
					if(roomAppearances.Count > 0)
					{
						mainDesc = $"{mainDesc}\n{string.Join(" ", roomAppearances.ToArray())}";
					}
				}
			}

			if(parent.HasComponent<RoomComponent>())
			{
				RoomComponent roomComp = (RoomComponent)parent.GetComponent<RoomComponent>();
				mainDesc = $"{mainDesc}\n{Colours.Fg(roomComp.GetExitString(), Colours.BoldCyan)}";
			}

			if(parent.HasComponent<PhysicsComponent>())
			{
				PhysicsComponent phys = (PhysicsComponent)parent.GetComponent<PhysicsComponent>();
				mainDesc = $"{mainDesc}\n{phys.GetExaminedSummary(viewer)}";
			}

			viewer.WriteLine(mainDesc);
			viewer.SendPrompt();
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			shortDescription =    reader[Text.FieldShortDesc].ToString();
			roomDescription =     reader[Text.FieldRoomDesc].ToString();
			examinedDescription = reader[Text.FieldExaminedDesc].ToString();
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", shortDescription);
			command.Parameters.AddWithValue("@p2", roomDescription);
			command.Parameters.AddWithValue("@p3", examinedDescription);	
		}
		internal override void ConfigureFromJson(JToken compData)
		{
			shortDescription =    (string)compData["shortdesc"];
			roomDescription =     (string)compData["roomdesc"];
			examinedDescription = (string)compData["examineddesc"];
		}
	}
}

