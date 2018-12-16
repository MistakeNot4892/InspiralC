using System.Data.SQLite;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using System;
using System.Linq;

namespace inspiral
{
	internal static partial class Components
	{
		internal const string Inventory = "inventory";
		internal static List<GameComponent> Inventories => GetComponents(Inventory);
	}

	internal static partial class Text
	{
		internal const string FieldEquippedSlots = "equipped";
	}
	internal class InventoryBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Components.Inventory;
		internal override GameComponent Build()
		{
			return new InventoryComponent();
		}
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_inventory WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = $@"components_inventory (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				{Text.FieldEquippedSlots} TEXT DEFAULT ''
				)";
		internal override string UpdateSchema   { get; set; } = $@"UPDATE components_inventory SET 
				{Text.FieldEquippedSlots} = @p1 
				WHERE id = @p0";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_inventory (
				id,
				{Text.FieldEquippedSlots}
				) VALUES (
				@p0, 
				@p1
				);";
	}
	class InventoryComponent : GameComponent
	{
		internal Dictionary<string, GameObject> carrying = new Dictionary<string, GameObject>();
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			foreach(KeyValuePair<string, long> equippedId in JsonConvert.DeserializeObject<Dictionary<string, long>>(reader[Text.FieldEquippedSlots].ToString()))
			{
				GameObject obj = (GameObject)Game.Objects.Get(equippedId.Value);
				if(obj != null)
				{
					carrying.Add(equippedId.Key.ToLower(), obj);
				}
			}
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			Dictionary<string, long> equippedById = new Dictionary<string, long>();
			foreach(KeyValuePair<string, GameObject> gameObj in carrying)
			{
				equippedById.Add(gameObj.Key, gameObj.Value.id);
			}
			command.Parameters.AddWithValue("@p1", JsonConvert.SerializeObject(equippedById));
		}
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
						$"You {removeMessage1p} {dropping.GetString(Components.Visible, Text.FieldShortDesc)}.",
						$"{Text.Capitalize(parent.GetString(Components.Visible, Text.FieldShortDesc))} {removeMessage3p} {dropping.GetString(Components.Visible, Text.FieldShortDesc)}."
						);
				}
				dropping.Move(parent.location);
			}
			return true;
		}
		internal List<string> GetWieldableSlots()
		{
			if(parent.HasComponent(Components.Mobile))
			{
				MobileComponent mob = (MobileComponent)parent.GetComponent(Components.Mobile);
				if(mob.bodyplan == null)
				{
					mob.SetBodyplan("humanoid");
				}
				return mob.bodyplan.graspers;
			}
			return new List<string>() { "hands" };
		}
		internal List<string> GetEquippableSlots()
		{
			if(parent.HasComponent(Components.Mobile))
			{
				MobileComponent mob = (MobileComponent)parent.GetComponent(Components.Mobile);
				if(mob.bodyplan == null)
				{
					mob.SetBodyplan("humanoid");
				}
				return mob.bodyplan.equipmentSlots;
			}
			return new List<string>() { "body" };
		}
		internal Tuple<string, string> GetSlotAndTokenFromInput(string input)
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
			return new Tuple<string, string>(objKey, objSlot);
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
			Tuple<string, string> tokens = GetSlotAndTokenFromInput(input);
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
					parent.ShowNearby(parent, 
						$"You pick up {equipping.GetString(Components.Visible, Text.FieldShortDesc)}.",
						$"{Text.Capitalize(parent.GetString(Components.Visible, Text.FieldShortDesc))} picks up {equipping.GetString(Components.Visible, Text.FieldShortDesc)}."
					);
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
			Tuple<string, string> tokens = GetSlotAndTokenFromInput(input);
			string slot = tokens.Item2;

			GameObject equipping = GetObjectFromInput(tokens.Item1, "equip");
			if(equipping != null)
			{
				if(!equipping.HasComponent(Components.Wearable))
				{
					parent.WriteLine("You cannot wear that.");
					return false;
				}
				WearableComponent worn = (WearableComponent)equipping.GetComponent(Components.Wearable);
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
				if(IsEquipped(equipping))
				{
					parent.WriteLine("You are already wearing that.");
					return false;
				}
				return Equip(equipping, slot, false);
			}
			return false;
		}
		internal bool TryToUnequip(string input)
		{
			GameObject unequipping = GetObjectFromInput(input, "remove");
			if(unequipping != null)
			{
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
						$"You equip {equipping.GetString(Components.Visible, Text.FieldShortDesc)} to your {slot}.",
						$"{Text.Capitalize(parent.GetString(Components.Visible, Text.FieldShortDesc))} equips {equipping.GetString(Components.Visible, Text.FieldShortDesc)} to {parent.gender.His} {slot}."
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
						$"You remove {unequipping.GetString(Components.Visible, Text.FieldShortDesc)} from your {removingSlot}.",
						$"{Text.Capitalize(parent.GetString(Components.Visible, Text.FieldShortDesc))} removes {unequipping.GetString(Components.Visible, Text.FieldShortDesc)} from {parent.gender.His} {removingSlot}."
					);
					Game.Objects.QueueForUpdate(parent);
				}
				return true;
			}
			return false;
		}
	}
}
