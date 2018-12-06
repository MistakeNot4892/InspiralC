using System.Data.SQLite;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace inspiral
{
	internal static partial class Components
	{
		internal const string Equipment = "equipment";
		internal static List<GameComponent> Equipments => GetComponents(Equipment);
	}

	internal static partial class Text
	{
		internal const string FieldEquippedSlots = "equipped";
		internal const string FieldEquippableSlots = "equippable_slots";
	}
	internal class EquipmentBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Components.Equipment;
		internal override GameComponent Build()
		{
			return new EquipmentComponent();
		}
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_equipment WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = $@"components_equipment (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE,
				{Text.FieldEquippedSlots} TEXT DEFAULT '',
				{Text.FieldEquippableSlots} TEXT DEFAULT ''
				)";
		internal override string UpdateSchema   { get; set; } = $@"UPDATE components_equipment SET 
				{Text.FieldEquippedSlots} = @p1, 
				{Text.FieldEquippableSlots} = @p2
				WHERE id = @p0";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_equipment (
				id,
				{Text.FieldEquippedSlots},
				{Text.FieldEquippableSlots}
				) VALUES (
				@p0,
				@p1,
				@p2
				);";	}

	class EquipmentComponent : GameComponent
	{
		Dictionary<string, GameObject> equipped = new Dictionary<string, GameObject>();
		List<string> equippableSlots = new List<string>() {"hands"};
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			foreach(KeyValuePair<string, long> equippedId in JsonConvert.DeserializeObject<Dictionary<string, long>>(reader[Text.FieldEquippedSlots].ToString()))
			{
				GameObject obj = (GameObject)Game.Objects.Get(equippedId.Value);
				if(obj != null)
				{
					equipped.Add(equippedId.Key.ToLower(), obj);
				}
			}
			equippableSlots = JsonConvert.DeserializeObject<List<string>>(reader[Text.FieldEquippableSlots].ToString());
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", JsonConvert.SerializeObject(equipped));
			command.Parameters.AddWithValue("@p2", JsonConvert.SerializeObject(equippableSlots));
		}
		internal bool Equip(GameObject equipper, GameObject equipment)
		{
			return Equip(equipper, equipment, "default");
		}
		internal bool Equip(GameObject equipper, GameObject equipment, string slot)
		{
			slot = slot.ToLower();
			if(slot == "default")
			{
				slot = equippableSlots[0];
			}
			if(equipped.ContainsKey(slot))
			{
				equipper.ShowMessage($"You already have {equipped[slot].GetString(Components.Visible, Text.FieldShortDesc)} equipped in your {slot}.");
				return false;
			}
			equipped.Add(slot, equipment);
			equipper.ShowNearby(equipper, 
				$"You equip {equipped[slot].GetString(Components.Visible, Text.FieldShortDesc)} to your {slot}.",
				$"{Text.Capitalize(equipper.GetString(Components.Visible, Text.FieldShortDesc))} equips {equipped[slot].GetString(Components.Visible, Text.FieldShortDesc)} to {equipper.gender.His} {slot}."
			);
			return true;
		}

		internal bool Unequip(GameObject equipper, GameObject equipment)
		{
			return Unequip(equipper, equipment, "default");
		}
		internal string GetSlot(GameObject equipment)
		{
			foreach(KeyValuePair<string, GameObject> equ in equipped)
			{
				if(equ.Value == equipment)
				{
					return equ.Key;
				}
			}
			return null;
		}
		internal bool Equipped(GameObject equipment)
		{
			return GetSlot(equipment) != null;
		}
		
		internal bool ForceUnequip(GameObject equipper, GameObject equipment)
		{
			string slot = GetSlot(equipment);
			{
				if(slot != null)
				{
					return Unequip(equipper, equipment, slot);
				}
			}
			return true;
		}

		internal bool Unequip(GameObject equipper, GameObject equipment, string slot)
		{
			slot = slot.ToLower();
			if(slot == "default")
			{
				slot = equippableSlots[0];
			}
			if(!equipped.ContainsKey(slot))
			{
				equipper.ShowMessage($"You do not have {equipment.GetString(Components.Visible, Text.FieldShortDesc)} equipped.");
				return false;
			}
			equipped.Remove(slot);
			equipper.ShowNearby(
				equipper,
				$"You unequip {equipment.GetString(Components.Visible, Text.FieldShortDesc)} from your {slot}.",
				$"{Text.Capitalize(equipper.GetString(Components.Visible, Text.FieldShortDesc))} unequips {equipment.GetString(Components.Visible, Text.FieldShortDesc)} from {equipper.gender.His} {slot}."
			);
			return true;
		}
	}
}
