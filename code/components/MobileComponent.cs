using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Mobiles =>  GetComponents<MobileComponent>();
	}

	internal static partial class Field
	{
		internal const string EnterMessage = "enter";
		internal const string LeaveMessage = "leave";
		internal const string DeathMessage = "death";
		internal const string Bodyplan =     "bodyplan";
		internal const string Species =      "species";
		internal const string BodypartList = "bodyparts";
	}

	internal class MobileBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(MobileComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Field.EnterMessage, (typeof(string), $"'{Text.DefaultEnterMessage}'", true, true) },
				{ Field.LeaveMessage, (typeof(string), $"'{Text.DefaultLeaveMessage}'", true, true) },
				{ Field.DeathMessage, (typeof(string), $"'{Text.DefaultDeathMessage}'", true, true) },
				{ Field.BodypartList, (typeof(string), "''", false, false) }
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
				case Field.EnterMessage:
					if(enterMessage != newValue)
					{
						enterMessage = newValue;
						success = true;
					}
					break;
				case Field.LeaveMessage:
					if(leaveMessage != newValue)
					{
						leaveMessage = newValue;
						success = true;
					}
					break;
				case Field.DeathMessage:
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
				case Field.EnterMessage:
					return enterMessage;
				case Field.LeaveMessage:
					return leaveMessage;
				case Field.DeathMessage:
					return deathMessage;
				case Field.Species:
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
					if(bp.GetBool(Field.CanGrasp) && !graspers.Contains(limb.Key))
					{
						graspers.Add(limb.Key);
					}
					if(bp.GetBool(Field.CanStand) && !stance.Contains(limb.Key))
					{
						stance.Add(limb.Key);
					}
					if(bp.GetBool(Field.NaturalWeapon) && !strikers.Contains(limb.Key))
					{
						strikers.Add(limb.Key);
					}
					foreach(string slot in bp.GetStringList(Field.EquipmentSlots))
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
			enterMessage = record.fields[Field.EnterMessage].ToString();
			leaveMessage = record.fields[Field.LeaveMessage].ToString();
			deathMessage = record.fields[Field.DeathMessage].ToString();
			foreach(KeyValuePair<string, long> limb in JsonConvert.DeserializeObject<Dictionary<string, long>>((string)record.fields[Field.BodypartList]))
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
				vis.SetValue(Field.ShortDesc, b.name);
				vis.SetValue(Field.RoomDesc, $"A severed {b.name} has been left here.");
				vis.SetValue(Field.ExaminedDesc, $"It is a severed {b.name} that has been lopped off its owner.");

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
				limbKeys.Add(limb.Key, limb.Value != null ? limb.Value.GetLong(Field.Id) : 0);
			}
			saveData.Add(Field.BodypartList, JsonConvert.SerializeObject(limbKeys));
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
