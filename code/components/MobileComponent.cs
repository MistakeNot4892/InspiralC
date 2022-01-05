using System.Data.SQLite;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Mobiles =>  GetComponents<MobileComponent>();
	}

	internal static partial class Text
	{
		internal const string FieldEnterMessage = "enter";
		internal const string FieldLeaveMessage = "leave";
		internal const string FieldDeathMessage = "death";
		internal const string FieldBodyplan =     "bodyplan";
		internal const string FieldSpecies =      "species";
		internal const string FieldBodypartList = "bodyparts";
	}

	internal class MobileBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(MobileComponent);
		}
		internal override List<string> editableFields { get; set; } = new List<string>() {Text.FieldEnterMessage, Text.FieldLeaveMessage, Text.FieldDeathMessage};
		internal override List<string> viewableFields { get; set; } = new List<string>() {Text.FieldEnterMessage, Text.FieldLeaveMessage, Text.FieldDeathMessage};
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_mobile WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = $@"CREATE TABLE IF NOT EXISTS components_mobile (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				{Text.FieldEnterMessage} TEXT DEFAULT '', 
				{Text.FieldLeaveMessage} TEXT DEFAULT '', 
				{Text.FieldDeathMessage} TEXT DEFAULT '',
				{Text.FieldBodypartList} TEXT DEFAULT ''
				);";
		internal override string UpdateSchema   { get; set; } = $@"UPDATE components_mobile SET 
				{Text.FieldEnterMessage} = @p1, 
				{Text.FieldLeaveMessage} = @p2, 
				{Text.FieldDeathMessage} = @p3,
				{Text.FieldBodypartList} = @p4
				WHERE id = @p0;";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_mobile (
				id,
				{Text.FieldEnterMessage},
				{Text.FieldLeaveMessage},
				{Text.FieldDeathMessage},
				{Text.FieldBodypartList}
				) VALUES (
				@p0, 
				@p1, 
				@p2, 
				@p3, 
				@p4
				);";
	}

	internal class MobileComponent : GameComponent
	{
		internal string enterMessage = Text.DefaultEnterMessage;
		internal string leaveMessage = Text.DefaultLeaveMessage;
		internal string deathMessage = Text.DefaultDeathMessage;
		internal string species =      "human";
		internal List<string> strikers = new List<string>();
		internal List<string> graspers = new List<string>();
		internal List<string> stance = new List<string>();
		internal List<string> equipmentSlots = new List<string>();

		internal Dictionary<string, GameEntity> limbs = new Dictionary<string, GameEntity>();
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
				case Text.FieldSpecies:
					return species;
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

			Bodyplan bp = null;
			if(!JsonExtensions.IsNullOrEmpty(compData["mobtype"]))
			{
				bp = Modules.Bodies.GetPlan((string)compData["mobtype"]);
			}
			if(bp == null)
			{
				bp = Modules.Bodies.GetPlan("humanoid");
			}

			foreach(Bodypart b in bp.allParts)
			{
				GameEntity newLimb = (GameEntity)Game.Objects.CreateNewInstance(false);
				newLimb.name = "limb";
				newLimb.aliases.Add("bodypart");

				VisibleComponent vis = (VisibleComponent)newLimb.AddComponent<VisibleComponent>();
				vis.SetValue(Text.FieldShortDesc, b.name);
				vis.SetValue(Text.FieldRoomDesc, $"A severed {b.name} has been left here.");
				vis.SetValue(Text.FieldExaminedDesc, $"It is a severed {b.name} that has been lopped off its owner.");

				PhysicsComponent phys = (PhysicsComponent)newLimb.AddComponent<PhysicsComponent>();
				phys.width =      b.width;
				phys.length =     b.length;
				phys.height =     b.height;
				phys.strikeArea = b.strikeArea;
				phys.edged =      b.isEdged;
				phys.UpdateValues();

				BodypartComponent body = (BodypartComponent)newLimb.AddComponent<BodypartComponent>();
				body.canGrasp = b.canGrasp;
				body.canStand = b.canStand;
				body.isNaturalWeapon = b.isNaturalWeapon;
				
				foreach(string s in b.equipmentSlots)
				{
					if(!body.equipmentSlots.Contains(s))
					{
						body.equipmentSlots.Add(s);
					}
				}
				Game.Objects.AddDatabaseEntry(newLimb);
				limbs.Add(b.name, newLimb);
			}
			UpdateLists();
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			enterMessage = reader[Text.FieldEnterMessage].ToString();
			leaveMessage = reader[Text.FieldLeaveMessage].ToString();
			deathMessage = reader[Text.FieldDeathMessage].ToString();
			foreach(KeyValuePair<string, long> limb in JsonConvert.DeserializeObject<Dictionary<string, long>>((string)reader[Text.FieldBodypartList]))
			{
				limbs.Add(limb.Key, (limb.Value != 0 ? (GameEntity)Game.Objects.GetByID(limb.Value) : null));
			}
		}

		internal override void FinalizeObjectLoad()
		{
			UpdateLists();
		}
		internal void UpdateLists()
		{
			graspers.Clear();
			strikers.Clear();
			stance.Clear();
			equipmentSlots.Clear();
			foreach(KeyValuePair<string, GameEntity> limb in limbs)
			{
				if(limb.Value != null)
				{
					BodypartComponent bp = (BodypartComponent)limb.Value.GetComponent<BodypartComponent>();
					if(bp.canGrasp && !graspers.Contains(limb.Key))
					{
						graspers.Add(limb.Key);
					}
					if(bp.canStand && !stance.Contains(limb.Key))
					{
						stance.Add(limb.Key);
					}
					if(bp.isNaturalWeapon && !strikers.Contains(limb.Key))
					{
						strikers.Add(limb.Key);
					}
					foreach(string slot in bp.equipmentSlots)
					{
						if(!equipmentSlots.Contains(slot))
						{
							equipmentSlots.Add(slot);
						}
					}
				}
			}
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", enterMessage);
			command.Parameters.AddWithValue("@p2", leaveMessage);
			command.Parameters.AddWithValue("@p3", deathMessage);
			Dictionary<string, long> limbKeys = new Dictionary<string, long>();
			foreach(KeyValuePair<string, GameEntity> limb in limbs)
			{
				limbKeys.Add(limb.Key, limb.Value != null ? limb.Value.id : 0);
			}
			command.Parameters.AddWithValue("@p4", JsonConvert.SerializeObject(limbKeys));
		}
		internal override string GetPrompt()
		{
			return $"{Colours.Fg("Pain:",parent.GetColour(Text.ColourDefaultPain))}{Colours.Fg("0%",parent.GetColour(Text.ColourDefaultPainHighlight))} {Colours.Fg("Bleed:",parent.GetColour(Text.ColourDefaultBleeding))}{Colours.Fg("0%",parent.GetColour(Text.ColourDefaultBleedingHighlight))}";
		}
		internal string GetWeightedRandomBodypart()
		{
			return limbs.ElementAt(Game.rand.Next(0, limbs.Count)).Key;
		}
	}
	internal class BodypartData
	{
		internal int painAmount = 0;
		internal bool isAmputated = false;
		internal bool isBroken = false;
		internal List<Wound> wounds = new List<Wound>();
	}
	internal class Wound
	{
		internal int severity = 0;
		internal int bleedAmount = 0;
		internal bool isOpen = false;
	}
}