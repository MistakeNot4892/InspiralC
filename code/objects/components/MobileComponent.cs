using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Mobiles =>  Repositories.Components.GetComponents<MobileComponent>();
	}

	internal static partial class Field
	{
		internal static DatabaseField EnterMessage = new DatabaseField(
			"enter", $"'{Text.DefaultEnterMessage}'", 
			typeof(string), true, true, false);
		internal static DatabaseField LeaveMessage = new DatabaseField(
			"leave", $"'{Text.DefaultLeaveMessage}'",
			typeof(string), true, true, false);
		internal static DatabaseField DeathMessage = new DatabaseField(
			"death", $"'{Text.DefaultDeathMessage}'",
			typeof(string), true, true, false);
		internal static DatabaseField Bodyplan = new DatabaseField(
			"bodyplan",  "",
			typeof(string), true, true, false);
		internal static DatabaseField Species = new DatabaseField(
			"species", "",
			typeof(string), true, true, false);
		internal static DatabaseField BodypartList = new DatabaseField(
			"bodyparts", "[]",
			typeof(string), true, true, true);
	}

	internal class MobileBuilder : GameComponentBuilder
	{
		public MobileBuilder()
		{
			ComponentType = typeof(MobileComponent);
			schemaFields = new List<DatabaseField>()
			{
				Field.Id,
				Field.Parent,
				Field.EnterMessage,
				Field.LeaveMessage,
				Field.DeathMessage,
				Field.BodypartList
			};
		}
		internal override GameComponent MakeComponent()
		{
			return new MobileComponent();
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
					var bpComp = limb.Value.GetComponent<BodypartComponent>();
					if(bpComp == null)
					{
						continue;
					}
					BodypartComponent bp = (BodypartComponent)bpComp;
					if(bp.GetValue<bool>(Field.CanGrasp) && !graspers.Contains(limb.Key))
					{
						graspers.Add(limb.Key);
					}
					if(bp.GetValue<bool>(Field.CanStand) && !stance.Contains(limb.Key))
					{
						stance.Add(limb.Key);
					}
					if(bp.GetValue<bool>(Field.NaturalWeapon) && !strikers.Contains(limb.Key))
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
			return $"{Colours.Fg("Pain:", GetColour(Text.ColourDefaultPain))}{Colours.Fg("0%", GetColour(Text.ColourDefaultPainHighlight))} {Colours.Fg("Bleed:", GetColour(Text.ColourDefaultBleeding))}{Colours.Fg("0%", GetColour(Text.ColourDefaultBleedingHighlight))}";
		}
		internal string GetWeightedRandomBodypart()
		{
			return limbs.ElementAt(Game.Random.Next(0, limbs.Count)).Key;
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
