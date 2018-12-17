using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using Newtonsoft.Json;

namespace inspiral
{

	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Rooms =>    GetComponents(Text.CompRoom);
	}

	internal static partial class Text
	{
		internal const string CompRoom =      "room";
		internal const string FieldExits = "exits";
	}

	internal class RoomBuilder : GameComponentBuilder
	{
		internal override List<string> viewableFields { get; set; } = new List<string>() {Text.FieldExits};
		internal override string Name         { get; set; } = Text.CompRoom;
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_room WHERE id = @p0;";

		internal override string TableSchema  { get; set; } = $@"components_room (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				{Text.FieldExits} TEXT DEFAULT ''
				)";
		internal override string UpdateSchema { get; set; } = $@"UPDATE components_room SET 
				{Text.FieldExits} = @p1 
				WHERE id = @p0;";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_room (
				id,
				{Text.FieldExits}
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
			exits = JsonConvert.DeserializeObject<Dictionary<string, long>>(reader[Text.FieldExits].ToString());
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", JsonConvert.SerializeObject(exits));
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
	}
}
