using System.Data.SQLite;
using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Components
	{
		internal const string Mobile = "mobile";
		internal static List<GameComponent> Mobiles =>  GetComponents(Mobile);
		}

	internal static partial class Text
	{
		internal const string FieldEnterMessage = "enter";
		internal const string FieldLeaveMessage = "leave";
		internal const string FieldDeathMessage = "death";
	}

	internal class MobileBuilder : GameComponentBuilder
	{
		internal override string Name         { get; set; } = Components.Mobile;
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_mobile WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = @"components_mobile (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				enterMessage TEXT DEFAULT '', 
				leaveMessage TEXT DEFAULT '', 
				deathMessage TEXT DEFAULT ''
				)";
		internal override string UpdateSchema   { get; set; } = @"UPDATE components_mobile SET 
				enterMessage = @p1, 
				leaveMessage = @p2, 
				deathMessage = @p3 
				WHERE id = @p0";
		internal override string InsertSchema { get; set; } = @"INSERT INTO components_mobile (
				id,
				enterMessage,
				leaveMessage,
				deathMessage
				) VALUES (
				@p0, 
				@p1, 
				@p2, 
				@p3 
				);";
		internal override GameComponent Build()
		{
			return new MobileComponent();
		}
	}

	internal class MobileComponent : GameComponent
	{
		internal string enterMessage = "A generic object enters from the $DIR.";
		internal string leaveMessage = "A generic object leaves to the $DIR.";
		internal string deathMessage = "A generic object lies here, dead.";
		internal override bool SetValue(string field, string newValue)
		{
			bool success = false;
			switch(field)
			{
				case Text.FieldEnterMessage:
					if(enterMessage != newValue)
					{
						enterMessage = newValue;
						success = true;
					}
					break;
				case Text.FieldLeaveMessage:
					if(leaveMessage != newValue)
					{
						leaveMessage = newValue;
						success = true;
					}
					break;
				case Text.FieldDeathMessage:
					if(deathMessage != newValue)
					{
						deathMessage = newValue;
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
				case Text.FieldEnterMessage:
					return enterMessage;
				case Text.FieldLeaveMessage:
					return leaveMessage;
				case Text.FieldDeathMessage:
					return deathMessage;
				default:
					return null;
			}
		}
		internal bool CanMove()
		{
			return true;
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			enterMessage = reader["enterMessage"].ToString();
			leaveMessage = reader["leaveMessage"].ToString();
			deathMessage = reader["deathMessage"].ToString();
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", enterMessage);
			command.Parameters.AddWithValue("@p2", leaveMessage);
			command.Parameters.AddWithValue("@p3", deathMessage);
		}
		internal override string GetStringSummary() 
		{
			string result =    $"enterMessage: {enterMessage}";
			result = $"{result}\nleaveMessage: {leaveMessage}";
			result = $"{result}\ndeathMessage: {deathMessage}";
			return result;
		}
	}
}