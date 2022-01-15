using System.Collections.Generic;
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
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Text.FieldShortDesc,    (typeof(string), "''", true, true)},
				{ Text.FieldRoomDesc,     (typeof(string), "''", true, true)},
				{ Text.FieldExaminedDesc, (typeof(string), "''", true, true)}
			};
			base.Initialize();
		}
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
			string mainDesc = $"{Colours.Fg(parent.GetShortDesc(), viewer.GetColour(Text.ColourDefaultHighlight))}.";
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
				mainDesc = $"{startingToken} {mainDesc}\n{theyAre} a {mob.species}";
				if(viewer == parent)
				{
					mainDesc += $". When people look at you, they see:\n{Text.Capitalize(parent.gender.They)} {parent.gender.Is} a {mob.species}";
				}
				if(examinedDescription == null || examinedDescription.Length <= 0)
				{
					mainDesc += ".";
				}
				else if(examinedDescription[0] == '.' || examinedDescription[0] == '!' || examinedDescription[0] == '?')
				{
					mainDesc += parent.ApplyStringTokens(examinedDescription);
				}
				else
				{
					mainDesc += $" {parent.ApplyStringTokens(examinedDescription)}";
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
				mainDesc += $"\n{Colours.Fg(parent.ApplyStringTokens(examinedDescription), parent.GetColour(Text.ColourDefaultSubtle))}";
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
				mainDesc = $"{mainDesc}\n{Colours.Fg(roomComp.GetExitString(), viewer.GetColour(Text.ColourDefaultExits))}";
			}

			if(parent.HasComponent<PhysicsComponent>())
			{
				PhysicsComponent phys = (PhysicsComponent)parent.GetComponent<PhysicsComponent>();
				mainDesc = $"{mainDesc}\n{phys.GetExaminedSummary(viewer)}";
			}
			viewer.WriteLine(mainDesc);
		}
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = new Dictionary<string, object>();
			saveData.Add(Text.FieldShortDesc,    shortDescription);
			saveData.Add(Text.FieldRoomDesc,     roomDescription);
			saveData.Add(Text.FieldExaminedDesc, examinedDescription);
			return saveData;
		}
		internal override void CopyFromRecord(DatabaseRecord record) 
		{
			base.CopyFromRecord(record);
			shortDescription =    record.fields[Text.FieldShortDesc].ToString();
			roomDescription =     record.fields[Text.FieldRoomDesc].ToString();
			examinedDescription = record.fields[Text.FieldExaminedDesc].ToString();
		}
	}
}
