using System.Data.SQLite;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Mobiles =>  GetComponents(Text.CompMobile);
	}

	internal static partial class Text
	{
		internal const string CompMobile = "mobile";
		internal const string FieldEnterMessage = "enter";
		internal const string FieldLeaveMessage = "leave";
		internal const string FieldDeathMessage = "death";
		internal const string FieldBodyplan =     "bodyplan";
		internal const string FieldRace =         "race";
	}

	internal class MobileBuilder : GameComponentBuilder
	{
		internal override List<string> editableFields { get; set; } = new List<string>() {Text.FieldEnterMessage, Text.FieldLeaveMessage, Text.FieldDeathMessage};
		internal override List<string> viewableFields { get; set; } = new List<string>() {Text.FieldEnterMessage, Text.FieldLeaveMessage, Text.FieldDeathMessage, Text.FieldBodyplan};
		internal override string Name         { get; set; } = Text.CompMobile;
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_mobile WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = $@"components_mobile (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				{Text.FieldEnterMessage} TEXT DEFAULT '', 
				{Text.FieldLeaveMessage} TEXT DEFAULT '', 
				{Text.FieldDeathMessage} TEXT DEFAULT '',
				{Text.FieldBodyplan} TEXT DEFAULT 'humanoid'
				)";
		internal override string UpdateSchema   { get; set; } = $@"UPDATE components_mobile SET 
				{Text.FieldEnterMessage} = @p1, 
				{Text.FieldLeaveMessage} = @p2, 
				{Text.FieldDeathMessage} = @p3, 
				{Text.FieldBodyplan} = @p4
				WHERE id = @p0";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_mobile (
				id,
				{Text.FieldEnterMessage},
				{Text.FieldLeaveMessage},
				{Text.FieldDeathMessage},
				{Text.FieldBodyplan}
				) VALUES (
				@p0, 
				@p1, 
				@p2, 
				@p3, 
				@p4
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
		internal string race =         "human";
		internal Bodyplan bodyplan;
		internal Dictionary<string, BodypartData> bodyData = new Dictionary<string, BodypartData>();
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
				case Text.FieldBodyplan:
					return bodyplan.name;
				case Text.FieldRace:
					return race;
				default:
					return null;
			}
		}
		internal bool CanMove()
		{
			return true;
		}
		internal override void ConfigureFromJson(JToken compData)
		{
			SetBodyplan((string)compData["mobtype"]);
			SetValue(Text.FieldEnterMessage, $"{parent.name} enters from the $DIR.");
			SetValue(Text.FieldLeaveMessage, $"{parent.name} leaves to the $DIR.");
			SetValue(Text.FieldDeathMessage, $"The corpse of {parent.name} lies here.");
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			enterMessage = reader[Text.FieldEnterMessage].ToString();
			leaveMessage = reader[Text.FieldLeaveMessage].ToString();
			deathMessage = reader[Text.FieldDeathMessage].ToString();
			bodyplan = Modules.Bodies.GetPlan(reader[Text.FieldBodyplan].ToString());
			UpdateBody();
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", enterMessage);
			command.Parameters.AddWithValue("@p2", leaveMessage);
			command.Parameters.AddWithValue("@p3", deathMessage);
			command.Parameters.AddWithValue("@p4", bodyplan.name);
		}
		internal void UpdateBody()
		{
			bodyData.Clear();
			foreach(Bodypart bp in bodyplan.allParts)
			{
				bodyData.Add(bp.name, new BodypartData());
			}
		}
		internal void SetBodyplan(string bPlan)
		{
			bodyplan = Modules.Bodies.GetPlan(bPlan);
			UpdateBody();
		}
		internal override string GetPrompt()
		{
			return $"{Colours.Fg("Pain:",Colours.Yellow)}{Colours.Fg("0%",Colours.BoldYellow)} {Colours.Fg("Bleed:",Colours.Red)}{Colours.Fg("0%",Colours.BoldRed)}";
		}
	}
	internal class BodypartData
	{
		int painAmount = 0;
		bool isAmputated = false;
		bool isBroken = false;
		List<Wound> wounds = new List<Wound>();
	}
	internal class Wound
	{
		int severity = 0;
		int bleedAmount = 0;
		bool isOpen = false;
	}
}