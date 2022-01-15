using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{

	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Rooms => GetComponents<RoomComponent>();
	}

	internal static partial class Text
	{
		internal const string FieldExits = "exits";
	}

	internal class RoomBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(RoomComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Text.FieldExits, (typeof(string), "''", true, false)}
			};
			base.Initialize();
		}
	}

	internal class RoomComponent : GameComponent
	{
		internal Dictionary<string, long> exits;

		internal override void Initialize()
		{
			exits = new Dictionary<string, long>();
		}
		internal string GetExitString()
		{
			if(exits.Count <= 0)
			{
				return "You cannot see any exits.";
			}
			else if(exits.Count == 1)
			{
				KeyValuePair<string, long> exit = exits.ElementAt(0);
				return $"You can see a single exit leading {exit.Key}.";
			}
			else
			{
				return $"You can see exits leading {Text.EnglishList(exits)}.";
			}
		}
		internal override string GetString(string field)
		{
			if(field == Text.FieldExits)
			{
				string exitString = null;
				if(exits.Count == 0)
				{
					exitString = "none";
				}
				else
				{
					exitString = "";
					foreach(KeyValuePair<string, long> exit in exits)
					{
						if(exitString != "")
						{
							exitString = $"{exitString}, ";
						}
						exitString += $"{exit.Key} ({exit.Value})";
					}
				}
				return exitString;
			}
			return null;
		}
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = base.GetSaveData();
			saveData.Add(Text.FieldExits, JsonConvert.SerializeObject(exits));
			return saveData;
		}
		internal override void CopyFromRecord(DatabaseRecord record) 
		{
			base.CopyFromRecord(record);
			exits = JsonConvert.DeserializeObject<Dictionary<string, long>>(record.fields[Text.FieldExits].ToString());			
		}
	}
}
