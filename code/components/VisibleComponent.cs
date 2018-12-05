using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace inspiral
{
	internal static partial class Components
	{
		internal const string Visible =   "visible";
		internal static List<GameComponent> Visibles => GetComponents(Visible);
	}

	internal static partial class Text
	{
		internal const string FieldShortDesc    = "short";
		internal const string FieldRoomDesc     =  "room";
		internal const string FieldExaminedDesc = "examined";
	}

	internal class VisibleBuilder : GameComponentBuilder
	{
		internal override List<string> validFields { get; set; } = new List<string>() {"short", "room", "examined"};
		internal override string Name         { get; set; } = Components.Visible;
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_visible WHERE id = @p0;";

		internal override string TableSchema  { get; set; } = @"components_visible (
			id INTEGER NOT NULL PRIMARY KEY UNIQUE,
			shortDescription TEXT DEFAULT '',
			roomDescription TEXT DEFAULT '',
			examinedDescription TEXT DEFAULT ''
			)";
		internal override string UpdateSchema { get; set; } = @"UPDATE components_visible SET 
			shortDescription = @p1, 
			roomDescription = @p2, 
			examinedDescription = @p3 
			WHERE id = @p0;";
		internal override string InsertSchema { get; set; } = @"INSERT INTO components_visible (
			id, 
			shortDescription, 
			roomDescription, 
			examinedDescription
			) VALUES (
			@p0, 
			@p1, 
			@p2, 
			@p3 
			);";
		internal override GameComponent Build()
		{
			return new VisibleComponent();
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
		internal void ExaminedBy(GameClient viewer, bool fromInside)
		{
			string mainDesc = $"{Colours.Fg(Text.Capitalize(shortDescription),Colours.BoldWhite)}.\n{Colours.Fg(examinedDescription, Colours.BoldBlack)}";
			if(parent != null && parent.contents.Count > 0)
			{
				List<string> roomAppearances = new List<string>();
				foreach(GameObject obj in parent.contents)
				{
					if(obj != viewer.shell && obj.HasComponent(Components.Visible))
					{
						roomAppearances.Add(obj.GetString(Components.Visible, Text.FieldRoomDesc));
					}
				}
				if(roomAppearances.Count > 0)
				{
					mainDesc = $"{mainDesc}\n{string.Join(" ", roomAppearances.ToArray())}";
				}
			}
			if(parent.HasComponent(Components.Room))
			{
				RoomComponent roomComp = (RoomComponent)parent.GetComponent(Components.Room);
				mainDesc = $"{mainDesc}\n{Colours.Fg(roomComp.GetExitString(), Colours.BoldCyan)}";
			}
			viewer.SendLineWithPrompt(mainDesc);
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			shortDescription =    reader["shortDescription"].ToString();
			roomDescription =     reader["roomDescription"].ToString();
			examinedDescription = reader["examinedDescription"].ToString();
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", shortDescription);
			command.Parameters.AddWithValue("@p2", roomDescription);
			command.Parameters.AddWithValue("@p3", examinedDescription);
		}
		internal override string GetStringSummary() 
		{
			string result =    $"shortDescription:    {shortDescription}";
			result = $"{result}\nroomDescription:     {roomDescription}";
			result = $"{result}\nexaminedDescription: {examinedDescription}";
			return result;
		}
		internal override string GetValueByField(string field) 
		{ 
			switch(field)
			{
				case "short":
					return GetString(Text.FieldShortDesc);
				case "room":
					return GetString(Text.FieldRoomDesc);
				case "examined":
					return GetString(Text.FieldExaminedDesc);
				default:
					return "Invalid field.";
			}
		}
		internal override string SetValueByField(string field, string value) 
		{
			switch(field)
			{
				case "short":
					SetValue(Text.FieldShortDesc, value);
					break;
				case "room":
					SetValue(Text.FieldRoomDesc, value);
					break;
				case "examined":
					SetValue(Text.FieldExaminedDesc, value);
					break;
				default:
					return "Invalid field.";
			}
			return null;
		}
	}
}

