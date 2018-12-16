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
		internal override List<string> editableFields { get; set; } = new List<string>() {Text.FieldShortDesc, Text.FieldRoomDesc, Text.FieldExaminedDesc};
		internal override List<string> viewableFields { get; set; } = new List<string>() {Text.FieldShortDesc, Text.FieldRoomDesc, Text.FieldExaminedDesc};
		internal override string Name         { get; set; } = Components.Visible;
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_visible WHERE id = @p0;";

		internal override string TableSchema  { get; set; } = $@"components_visible (
			id INTEGER NOT NULL PRIMARY KEY UNIQUE,
			{Text.FieldShortDesc} TEXT DEFAULT '',
			{Text.FieldRoomDesc} TEXT DEFAULT '',
			{Text.FieldExaminedDesc} TEXT DEFAULT ''
			)";
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
			string mainDesc = $"{Colours.Fg(Text.Capitalize(shortDescription),Colours.BoldWhite)}.";
			if(parent.HasComponent(Components.Mobile))
			{
				string startingToken = (parent == viewer.shell) ? "You're" : "That's";
				MobileComponent mob = (MobileComponent)parent.GetComponent(Components.Mobile);
				mainDesc = $"{startingToken} {mainDesc}\n{Text.Capitalize(parent.gender.He)} {parent.gender.Is} a {mob.race}";
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
					mainDesc += $"\n{Text.Capitalize(parent.gender.He)} {parent.gender.Is} carrying:";
					foreach(string line in clothing)
					{
						mainDesc += $"\n{Text.Capitalize(line)}";
					}
				}
				else
				{
					mainDesc += $"\n{Text.Capitalize(parent.gender.He)} {parent.gender.Is} completely naked.";
				}
				mainDesc = Text.FormatProse(mainDesc);
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
			if(parent.HasComponent(Components.Room))
			{
				RoomComponent roomComp = (RoomComponent)parent.GetComponent(Components.Room);
				mainDesc = $"{mainDesc}\n{Colours.Fg(roomComp.GetExitString(), Colours.BoldCyan)}";
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
	}
}

