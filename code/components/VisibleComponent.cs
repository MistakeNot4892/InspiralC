using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace inspiral
{
	internal class VisibleComponent : GameComponent
	{
		internal string shortDescription = "a generic object";
		internal string roomDescription = "A generic object is here.";
		internal string examinedDescription = "This is a generic object. Fascinating stuff.";
		internal VisibleComponent()
		{
			key = Components.Visible;
		}
		internal override void SetValue(int field, string newValue)
		{
			switch(field)
			{
				case Text.FieldShortDesc:
					shortDescription = newValue;
					break;
				case Text.FieldRoomDesc:
					roomDescription = newValue;
					break;
				case Text.FieldExaminedDesc:
					examinedDescription = newValue;
					break;
			}
		}
		internal override string GetStringValue(int field)
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
			viewer.WriteLinePrompted(mainDesc);
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
	}
}

