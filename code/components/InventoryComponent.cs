using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Inventories => GetComponents<InventoryComponent>();
	}

	internal static partial class Text
	{
		internal const string FieldEquippedSlots = "equipped";
	}
	internal class InventoryBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(InventoryComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Text.FieldEquippedSlots, (typeof(string), "''", false, false) }
			};
			base.Initialize();
		}
	}
	class InventoryComponent : GameComponent
	{
		internal Dictionary<string, GameObject> carrying = new Dictionary<string, GameObject>();
		internal GameObject GetObjectFromInput(string input, string action)
		{
			string inputRaw = input;
			input = input.ToLower().Trim();
			if(input == "")
			{
				parent.WriteLine($"What do you wish to {action}?");
				return null;
			}
			GameObject returning = parent.FindGameObjectInContents(input);
			if(returning == null)
			{
				if(carrying.ContainsKey(input))
				{
					returning = carrying[input];
				}
			}
			if(returning == null)
			{
				parent.WriteLine($"You cannot find '{inputRaw}' amongst your possessions.");
			}
			return returning;
		}
		internal bool TryToDrop(string input) 
		{
			GameObject tryingToDrop = GetObjectFromInput(input, "drop");
			if(tryingToDrop != null)
			{
				return Drop(tryingToDrop, false);
			}
			return false;
		}
		internal bool Drop(GameObject dropping, bool silent)
		{
			if(parent.location == null)
			{
				if(!silent)
				{
					parent.WriteLine("There is nowhere to drop that.");
				}
				return false;
			}
			string removeMessage1p = "drop";
			string removeMessage3p = "drops";

			if(IsEquipped(dropping))
			{
				if(!CanUnequip(dropping))
				{
					if(!silent)
					{
						parent.WriteLine("You cannot drop that.");
					}
					return false;
				}
				removeMessage1p = "remove and drop";
				removeMessage3p = "removes and drops";
			}
			string removingSlot = null;
			foreach(KeyValuePair<string, GameObject> thing in carrying)
			{
				if(thing.Value == dropping)
				{
					removingSlot = thing.Key;
					break;
				}
			}
			if(removingSlot != null)
			{
				carrying.Remove(removingSlot);
				if(!silent)
				{
					parent.ShowNearby(parent, 
						$"You {removeMessage1p} {dropping.GetShortDesc()}.",
						$"{Text.Capitalize(parent.GetShortDesc())} {removeMessage3p} {dropping.GetShortDesc()}."
						);
				}
				dropping.Move(parent.location);
			}
			return true;
		}
		internal List<string> GetWieldableSlots()
		{
			if(parent.HasComponent<MobileComponent>())
			{
				MobileComponent mob = (MobileComponent)parent.GetComponent<MobileComponent>();
				return mob.graspers;
			}
			return new List<string>() { "hands" };
		}
		internal List<string> GetEquippableSlots()
		{
			if(parent.HasComponent<MobileComponent>())
			{
				MobileComponent mob = (MobileComponent)parent.GetComponent<MobileComponent>();
				return mob.equipmentSlots;
			}
			return new List<string>() { "body" };
		}
		internal System.Tuple<string, string> GetSlotAndTokenFromInput(string input)
		{
			string objKey = input.ToLower().Trim();
			string objSlot = "default";
			string[] tokens = null;
			foreach(string splitpoint in new List<string>() { "on", "to", "in", "from", "as", "with"})
			{
				string splittoken = $" {splitpoint} ";
				if(objKey.Contains(splittoken))
				{
					tokens = objKey.Split(splittoken);
					if(tokens.Length >= 2)
					{
						break;
					}
					else
					{
						tokens = null;
					}
				}
			}
			if(tokens != null && tokens.Length > 1)
			{
				objKey = tokens[0];
				objSlot = tokens[1];
			}
			else
			{
				tokens = objKey.Split(" ");
				objKey = tokens[0];
				if(tokens.Length > 1)
				{
					objSlot = tokens[1];
					if(tokens.Length > 2)
					{
						for(int i = 2;i<tokens.Length;i++)
						{
							objSlot += $" {tokens[i]}";
						}
					}
				}

			}
			return new System.Tuple<string, string>(objKey, objSlot);
		}
		internal bool IsWielded(GameObject equipping)
		{
			foreach(string slot in GetWieldableSlots())
			{
				if(carrying.ContainsKey(slot))
				{
					if(carrying[slot] == equipping)
					{
						return true;
					}
				}
			}
			return false;
		}
		internal bool IsEquipped(GameObject equipping)
		{
			foreach(string slot in GetEquippableSlots())
			{
				if(carrying.ContainsKey(slot))
				{
					if(carrying[slot] == equipping)
					{
						return true;
					}
				}
			}
			return false;
		}
		internal bool TryToCollect(string input)
		{
			System.Tuple<string, string> tokens = GetSlotAndTokenFromInput(input);
			if(tokens.Item1 == "")
			{
				parent.WriteLine("What do you wish to pick up?");
				return false;
			}
			GameObject equipping = parent.FindGameObjectNearby(tokens.Item1);
			if(equipping != null)
			{
				bool success = false;
				string slot = tokens.Item2;
				if(slot == null || slot == "default")
				{
					success = PutInHands(equipping);
				}
				else
				{
					success = PutInHands(equipping, slot);
				}
				if(success)
				{
					Game.Objects.QueueForUpdate(parent);
					string collectionMessage1p = $"You pick up {equipping.GetShortDesc()}";
					string collectionMessage3p = $"{Text.Capitalize(parent.GetShortDesc())} picks up {equipping.GetShortDesc()}";
					if(slot != null && slot != "default")
					{
						collectionMessage1p = $"{collectionMessage1p} with your {slot}";
						collectionMessage3p = $"{collectionMessage1p} with {parent.gender.Their} {slot}";
					}
					parent.ShowNearby(parent, $"{collectionMessage1p}.", $"{collectionMessage3p}.");
					return true;
				}
			}
			else
			{
				parent.WriteLine($"You cannot see '{tokens.Item1}' here.");
			}
			return false;
		}
		internal bool TryToEquip(string input) 
		{
			System.Tuple<string, string> tokens = GetSlotAndTokenFromInput(input);
			string slot = tokens.Item2;

			GameObject equipping = GetObjectFromInput(tokens.Item1, "equip");
			if(equipping != null)
			{
				if(!equipping.HasComponent<WearableComponent>())
				{
					parent.WriteLine("You cannot wear that.");
					return false;
				}
				WearableComponent worn = (WearableComponent)equipping.GetComponent<WearableComponent>();
				if(slot == null || slot == "default")
				{
					slot = worn.wearableSlots.FirstOrDefault();
				}
				if(!worn.wearableSlots.Contains(slot))
				{
					parent.WriteLine($"You cannot wear that on your {slot}.");
					return false;
				}
				if(!GetEquippableSlots().Contains(slot))
				{
					parent.WriteLine($"You cannot equip anything to your {slot}.");
					return false;
				}
				if(carrying.ContainsKey(slot))
				{
					parent.WriteLine($"You are already wearing something on your {slot}.");
					return false;
				}
				if(IsEquipped(equipping))
				{
					parent.WriteLine("You are already wearing that.");
					return false;
				}
				return Equip(equipping, slot, false);
			}
			return false;
		}
		// TODO make it prioritize actually equipped items before held items.
		internal bool TryToUnequip(string input)
		{
			GameObject unequipping = GetObjectFromInput(input, "remove");
			if(unequipping != null)
			{
				if(!IsEquipped(unequipping))
				{
					parent.WriteLine($"You are not currently wearing that.");
					return false;
				}
				return Unequip(unequipping);
			}
			return false;
		}
		internal bool PutInHands(GameObject thing, string slot)
		{
			return (!carrying.ContainsKey(slot) && Equip(thing, slot, true));
		}
		internal bool PutInHands(GameObject thing)
		{
			foreach(string slot in GetWieldableSlots())
			{
				if(PutInHands(thing, slot))
				{
					return true;
				}
			}
			parent.WriteLine("Your hands are full.");
			return false;
		}
		internal bool Equip(GameObject equipping, string slot, bool silent) 
		{
			if(!carrying.ContainsKey(slot))
			{
				if(equipping.location != parent)
				{
					equipping.Move(parent);
				}
				foreach(string otherSlot in GetWieldableSlots())
				{
					if(carrying.ContainsKey(otherSlot) && carrying[otherSlot] == equipping)
					{
						carrying.Remove(otherSlot);
					}
				}
				carrying.Add(slot, equipping);
				if(!silent)
				{
					parent.ShowNearby(parent, 
						$"You equip {equipping.GetShortDesc()} to your {slot}.",
						$"{Text.Capitalize(parent.GetShortDesc())} equips {equipping.GetShortDesc()} to {parent.gender.Their} {slot}."
					);
				}
				Game.Objects.QueueForUpdate(parent);
				return true;
			}
			return false; 
		}

		internal bool CanUnequip(GameObject thing)
		{
			return true;
		}
		internal bool Unequip(GameObject unequipping) 
		{
			string removingSlot = null;
			foreach(KeyValuePair<string, GameObject> thing in carrying)
			{
				if(thing.Value == unequipping)
				{
					removingSlot = thing.Key;
					break;
				}
			}
			if(removingSlot != null && carrying.ContainsKey(removingSlot))
			{
				if(CanUnequip(unequipping) && PutInHands(unequipping))
				{
					foreach(string otherSlot in GetEquippableSlots())
					{
						if(carrying.ContainsKey(otherSlot) && carrying[otherSlot] == unequipping)
						{
							carrying.Remove(otherSlot);
						}
					}
					parent.ShowNearby(parent, 
						$"You remove {unequipping.GetShortDesc()} from your {removingSlot}.",
						$"{Text.Capitalize(parent.GetShortDesc())} removes {unequipping.GetShortDesc()} from {parent.gender.Their} {removingSlot}."
					);
					Game.Objects.QueueForUpdate(parent);
				}
				return true;
			}
			return false;
		}
		internal override void CopyFromRecord(DatabaseRecord record) 
		{
			base.CopyFromRecord(record);
			foreach(KeyValuePair<string, long> equippedId in JsonConvert.DeserializeObject<Dictionary<string, long>>(record.fields[Text.FieldEquippedSlots].ToString()))
			{
				GameObject obj = (GameObject)Game.Objects.GetByID(equippedId.Value);
				if(obj != null && obj.location == parent)
				{
					carrying.Add(equippedId.Key.ToLower(), obj);
				}
			}
		}
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = new Dictionary<string, object>();
			Dictionary<string, long> equippedById = new Dictionary<string, long>();
			foreach(KeyValuePair<string, GameObject> gameObj in carrying)
			{
				equippedById.Add(gameObj.Key, gameObj.Value.id);
			}
			saveData.Add(Text.FieldEquippedSlots, JsonConvert.SerializeObject(equippedById));
			return saveData;
		}
	}
}
