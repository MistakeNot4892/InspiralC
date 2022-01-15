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
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Text.FieldEnterMessage, (typeof(string), $"'{Text.DefaultEnterMessage}'", true, true) },
				{ Text.FieldLeaveMessage, (typeof(string), $"'{Text.DefaultLeaveMessage}'", true, true) },
				{ Text.FieldDeathMessage, (typeof(string), $"'{Text.DefaultDeathMessage}'", true, true) },
				{ Text.FieldBodypartList, (typeof(string), "''", false, false) }
			};
			base.Initialize();
		}
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
		internal Dictionary<string, GameObject> limbs = new Dictionary<string, GameObject>();
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
			foreach(KeyValuePair<string, GameObject> limb in limbs)
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
		internal override string GetPrompt()
		{
			return $"{Colours.Fg("Pain:",parent.GetColour(Text.ColourDefaultPain))}{Colours.Fg("0%",parent.GetColour(Text.ColourDefaultPainHighlight))} {Colours.Fg("Bleed:",parent.GetColour(Text.ColourDefaultBleeding))}{Colours.Fg("0%",parent.GetColour(Text.ColourDefaultBleedingHighlight))}";
		}
		internal string GetWeightedRandomBodypart()
		{
			return limbs.ElementAt(Game.rand.Next(0, limbs.Count)).Key;
		}
		internal override void CopyFromRecord(DatabaseRecord record) 
		{
			base.CopyFromRecord(record);
			enterMessage = record.fields[Text.FieldEnterMessage].ToString();
			leaveMessage = record.fields[Text.FieldLeaveMessage].ToString();
			deathMessage = record.fields[Text.FieldDeathMessage].ToString();
			foreach(KeyValuePair<string, long> limb in JsonConvert.DeserializeObject<Dictionary<string, long>>((string)record.fields[Text.FieldBodypartList]))
			{
				limbs.Add(limb.Key, (limb.Value != 0 ? (GameObject)Game.Objects.GetByID(limb.Value) : null));
			}
			/*
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
				GameObject newLimb = (GameObject)Game.Objects.CreateNewInstance();
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
				limbs.Add(b.name, newLimb);
			}
			*/
		}
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = base.GetSaveData();
			Dictionary<string, long> limbKeys = new Dictionary<string, long>();
			foreach(KeyValuePair<string, GameObject> limb in limbs)
			{
				limbKeys.Add(limb.Key, limb.Value != null ? limb.Value.id : 0);
			}
			saveData.Add(Text.FieldBodypartList, JsonConvert.SerializeObject(limbKeys));
			return saveData;
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
