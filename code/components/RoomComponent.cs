using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using Newtonsoft.Json;

namespace inspiral
{

	internal static partial class Components
	{
		internal const string Room =      "room";
		internal static List<GameComponent> Rooms =>    GetComponents(Room);
	}

	internal class RoomBuilder : GameComponentBuilder
	{
		internal override string Name         { get; set; } = Components.Room;
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_room WHERE id = @p0;";

		internal override string TableSchema  { get; set; } = @"components_room (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				exits TEXT DEFAULT ''
				)";
		internal override string UpdateSchema { get; set; } = @"UPDATE components_room SET 
				exits = @p1 
				WHERE id = @p0;";
		internal override string InsertSchema { get; set; } = @"INSERT INTO components_room (
				id,
				exits
				) VALUES (
				@p0,
				@p1
				);";
		internal override GameComponent Build()
		{
			return new RoomComponent();
		}
	}

	internal class RoomComponent : GameComponent
	{
		internal Dictionary<string, long> exits;

		internal RoomComponent()
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
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			exits = JsonConvert.DeserializeObject<Dictionary<string, long>>(reader["exits"].ToString());
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", JsonConvert.SerializeObject(exits));
		}
		internal override string GetStringSummary() 
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
			return $"exits: {exitString}";
		}

	}
}
