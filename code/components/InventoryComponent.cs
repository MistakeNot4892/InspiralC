using System.Data.SQLite;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using System;

namespace inspiral
{
	internal static partial class Components
	{
		internal const string Inventory = "inventory";
		internal static List<GameComponent> Inventories => GetComponents(Inventory);
	}

	internal static partial class Text
	{
		internal const string FieldInventoryContents = "inventory_contents";
		internal const string FieldInventoryCapacity = "max_capacity";
		internal const string FieldEquippedSlots = "equipped";
		internal const string FieldWieldedSlots = "wielding";
	}
	internal class InventoryBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Components.Inventory;
		internal override List<string> viewableFields { get; set; } = new List<string>() { Text.FieldInventoryCapacity };
		internal override GameComponent Build()
		{
			return new InventoryComponent();
		}
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_inventory WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = $@"components_inventory (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				{Text.FieldInventoryContents} TEXT DEFAULT '', 
				{Text.FieldInventoryCapacity} INTEGER DEFAULT 10,
				{Text.FieldEquippedSlots} TEXT DEFAULT '',
				{Text.FieldWieldedSlots} TEXT DEFAULT ''
				)";
		internal override string UpdateSchema   { get; set; } = $@"UPDATE components_inventory SET 
				{Text.FieldInventoryContents} = @p1, 
				{Text.FieldInventoryCapacity} = @p2,
				{Text.FieldEquippedSlots} =     @p3,
				{Text.FieldWieldedSlots} =      @p4
				WHERE id = @p0";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_inventory (
				id,
				{Text.FieldInventoryContents},
				{Text.FieldInventoryCapacity},
				{Text.FieldEquippedSlots},
				{Text.FieldWieldedSlots}
				) VALUES (
				@p0, 
				@p1, 
				@p2,
				@p3,
				@p4
				);";	}

	class InventoryComponent : GameComponent
	{
		internal long maxCapacity = 10;
		internal List<GameObject> contents = new List<GameObject>();
		internal Dictionary<string, GameObject> equipped = new Dictionary<string, GameObject>();
		internal Dictionary<string, GameObject> wielded = new Dictionary<string, GameObject>();
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			foreach(long objId in JsonConvert.DeserializeObject<List<long>>(reader[Text.FieldInventoryContents].ToString()))
			{
				GameObject addingObj = (GameObject)Game.Objects.Get(objId);
				if(addingObj != null)
				{
					contents.Add(addingObj);
				}
			}
			maxCapacity = (long)reader[Text.FieldInventoryCapacity];
			foreach(KeyValuePair<string, long> equippedId in JsonConvert.DeserializeObject<Dictionary<string, long>>(reader[Text.FieldEquippedSlots].ToString()))
			{
				GameObject obj = (GameObject)Game.Objects.Get(equippedId.Value);
				if(obj != null)
				{
					equipped.Add(equippedId.Key.ToLower(), obj);
				}
			}
			foreach(KeyValuePair<string, long> wieldedId in JsonConvert.DeserializeObject<Dictionary<string, long>>(reader[Text.FieldWieldedSlots].ToString()))
			{
				GameObject obj = (GameObject)Game.Objects.Get(wieldedId.Value);
				if(obj != null)
				{
					wielded.Add(wieldedId.Key.ToLower(), obj);
				}
			}
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);

			List<long> invKeys = new List<long>();
			foreach(GameObject obj in contents)
			{
				invKeys.Add(obj.id);
			}
			command.Parameters.AddWithValue("@p1", JsonConvert.SerializeObject(invKeys));
			command.Parameters.AddWithValue("@p2", maxCapacity);
			Dictionary<string, long> equippedById = new Dictionary<string, long>();
			foreach(KeyValuePair<string, GameObject> gameObj in equipped)
			{
				equippedById.Add(gameObj.Key, gameObj.Value.id);
			}
			command.Parameters.AddWithValue("@p3", JsonConvert.SerializeObject(equippedById));
			Dictionary<string, long> wieldedById = new Dictionary<string, long>();
			foreach(KeyValuePair<string, GameObject> gameObj in wielded)
			{
				wieldedById.Add(gameObj.Key, gameObj.Value.id);
			}
			command.Parameters.AddWithValue("@p4", JsonConvert.SerializeObject(wieldedById));
		}
		internal override string GetString(string field)
		{
			if(field == Text.FieldInventoryCapacity)
			{
				return $"{maxCapacity}";
			}
			return null;
		}
		internal bool Collect(GameObject collector, GameObject collecting)
		{
			if(contents.Count+1 >= maxCapacity)
			{
				collector.ShowMessage("You are too overburdened to hold another object.");
				return false;
			}
			if(!collecting.Collectable(collector))
			{
				collector.ShowMessage("You cannot pick that up.");
				return false;
			}
			collector.ShowNearby(collector, 
				$"You pick up {collecting.GetString(Components.Visible, Text.FieldShortDesc)}.",
				$"You picks up {collecting.GetString(Components.Visible, Text.FieldShortDesc)}."
				);
			collecting.Move(collector);
			contents.Add(collecting);
			return true;
		}

		internal GameObject GetObjectFromInput(string input)
		{
			string inputRaw = input;
			input = input.ToLower().Trim();
			GameObject returning = parent.FindGameObjectInContents(input);
			if(returning == null)
			{
				if(wielded.ContainsKey(input))
				{
					returning = wielded[input];
				}
				else if(equipped.ContainsKey(input))
				{
					returning = equipped[input];
				}
			}
			if(returning == null)
			{
				parent.ShowMessage($"You cannot find '{inputRaw}' amongst your possessions.");
			}
			return returning;
		}
		internal bool TryToDrop(string input) 
		{
			GameObject tryingToDrop = GetObjectFromInput(input);
			if(tryingToDrop != null)
			{
				return Drop(tryingToDrop);
			}
			return false;
		}
		internal bool Drop(GameObject dropping)
		{
			if(parent.location == null)
			{
				parent.ShowMessage("There is nowhere to drop that.");
				return false;
			}
			if((IsEquipped(dropping) && !Unequip(dropping)) || (IsWielded(dropping) && !Unwield(dropping)))
			{
				parent.ShowMessage("You cannot drop that.");
				return false;
			}
			parent.ShowNearby(parent, 
				$"You drop {dropping.GetString(Components.Visible, Text.FieldShortDesc)}.",
				$"{Text.Capitalize(parent.GetString(Components.Visible, Text.FieldShortDesc))} drops {dropping.GetString(Components.Visible, Text.FieldShortDesc)}."
				);
			dropping.Move(parent.location);
			contents.Remove(dropping);
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
			foreach(string splitpoint in new List<string>() { "on", "to", "in", "from", "as" })
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
			foreach(KeyValuePair<string, GameObject> entry in equipped)
			{
				if(entry.Value == equipping)
				{
					parent.ShowMessage("You are wearing that currently.");
					return true;
				}
			}
			return false;
		}
		internal bool IsWielded(GameObject wielding)
		{
			foreach(KeyValuePair<string, GameObject> entry in wielded)
			{
				if(entry.Value == wielding)
				{
					parent.ShowMessage("You are wielding that currently.");
					return true;
				}
			}
			return false;
		}
		internal bool TryToEquip(string input) 
		{
			Tuple<string, string> tokens = GetSlotAndTokenFromInput(input);
			string slot = tokens.Item2;
			if(slot == null || slot == "default")
			{
				slot = GetEquippableSlots()[0];
			}
			if(!GetEquippableSlots().Contains(slot))
			{
				parent.ShowMessage($"You cannot equip anything to your {slot}.");
				return false;
			}
			GameObject equipping = GetObjectFromInput(tokens.Item1);
			if(equipping != null && !IsWielded(equipping) && !IsEquipped(equipping))
			{
				return Equip(equipping, slot);
			}
			return false;
		}
		internal bool TryToWield(string input) 
		{
			Tuple<string, string> tokens = GetSlotAndTokenFromInput(input);
			string slot = tokens.Item2;
			if(slot == null || slot == "default")
			{
				slot = GetWieldableSlots()[0];
			}
			if(!GetWieldableSlots().Contains(slot))
			{
				parent.ShowMessage($"You cannot wield anything in your {slot}.");
				return false;
			}
			GameObject wielding = parent.FindGameObjectInContents(tokens.Item1);
			if(wielding != null && !IsWielded(wielding) && !IsEquipped(wielding))
			{
				return Wield(wielding, slot);
			}
			return false;
		}

		internal bool TryToUnequip(string input)
		{
			GameObject unequipping = GetObjectFromInput(input);
			if(unequipping != null)
			{
				return Unequip(unequipping);
			}
			return false;
		}
		internal bool TryToUnwield(string input)
		{
			GameObject unwielding = GetObjectFromInput(input);
			if(unwielding != null)
			{
				return Unwield(unwielding);
			}
			return false;
		}
		internal bool Equip(GameObject equipping, string slot) 
		{
			if(!equipped.ContainsKey(slot))
			{
				equipped.Add(slot, equipping);
				parent.ShowNearby(parent, 
					$"You begin wearing {equipping.GetString(Components.Visible, Text.FieldShortDesc)} on your {slot}.",
					$"{Text.Capitalize(parent.GetString(Components.Visible, Text.FieldShortDesc))} begins wearing {equipping.GetString(Components.Visible, Text.FieldShortDesc)} on {parent.gender.His} {slot}."
				);
				Game.Objects.QueueForUpdate(parent);
				return true;
			}
			return false; 
		}
		internal bool Unequip(GameObject unequipping) 
		{
			string removingSlot = null;
			foreach(KeyValuePair<string, GameObject> thing in equipped)
			{
				if(thing.Value == unequipping)
				{
					removingSlot = thing.Key;
					break;
				}
			}
			if(removingSlot != null && equipped.ContainsKey(removingSlot))
			{
				equipped.Remove(removingSlot);
				parent.ShowNearby(parent, 
					$"You remove {unequipping.GetString(Components.Visible, Text.FieldShortDesc)} from your {removingSlot}.",
					$"{Text.Capitalize(parent.GetString(Components.Visible, Text.FieldShortDesc))} removes {unequipping.GetString(Components.Visible, Text.FieldShortDesc)} from {parent.gender.His} {removingSlot}."
				);
				Game.Objects.QueueForUpdate(parent);
				return true;
			}
			return false;
		}
		internal bool Wield(GameObject wielding, string slot) 
		{
			if(!wielded.ContainsKey(slot))
			{
				wielded.Add(slot, wielding);
				parent.ShowNearby(parent, 
					$"You begin to wield {wielding.GetString(Components.Visible, Text.FieldShortDesc)} in your {slot}.",
					$"{Text.Capitalize(parent.GetString(Components.Visible, Text.FieldShortDesc))} begins to wield {wielding.GetString(Components.Visible, Text.FieldShortDesc)} in {parent.gender.His} {slot}."
				);
				Game.Objects.QueueForUpdate(parent);
				return true;
			}
			return false; 
		}
		internal bool Unwield(GameObject unwielding) 
		{
			string removingSlot = null;
			foreach(KeyValuePair<string, GameObject> thing in wielded)
			{
				if(thing.Value == unwielding)
				{
					removingSlot = thing.Key;
					break;
				}
			}
			if(removingSlot != null && wielded.ContainsKey(removingSlot))
			{
				wielded.Remove(removingSlot);
				parent.ShowNearby(parent, 
					$"You cease wielding {unwielding.GetString(Components.Visible, Text.FieldShortDesc)} in your {removingSlot}.",
					$"{Text.Capitalize(parent.GetString(Components.Visible, Text.FieldShortDesc))} ceases wielding {unwielding.GetString(Components.Visible, Text.FieldShortDesc)} in {parent.gender.His} {removingSlot}."
				);
				Game.Objects.QueueForUpdate(parent);
				return true;
			}
			return false;
		}
	}
}
