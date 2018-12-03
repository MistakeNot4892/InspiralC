using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using Newtonsoft.Json;

namespace inspiral
{
	internal class RoomComponent : GameComponent
	{
		internal Dictionary<string, long> exits;

		internal RoomComponent()
		{
			key = Components.Room;
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
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			exits = JsonConvert.DeserializeObject<Dictionary<string, long>>(reader["exits"].ToString());
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", JsonConvert.SerializeObject(exits));
		}
	}
}
