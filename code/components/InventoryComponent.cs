using System.Data.SQLite;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;

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
				{Text.FieldInventoryCapacity} INTEGER DEFAULT 10
				)";
		internal override string UpdateSchema   { get; set; } = $@"UPDATE components_inventory SET 
				{Text.FieldInventoryContents} = @p1, 
				{Text.FieldInventoryCapacity} = @p2
				WHERE id = @p0";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_inventory (
				id,
				{Text.FieldInventoryContents},
				{Text.FieldInventoryCapacity}
				) VALUES (
				@p0, 
				@p1, 
				@p2 
				);";	}

	class InventoryComponent : GameComponent
	{
		internal long maxCapacity = 10;
		internal List<GameObject> contents = new List<GameObject>();
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
		internal bool Drop(GameObject dropper, GameObject dropInto, GameObject dropping)
		{
			if(dropInto == null)
			{
				dropper.ShowMessage("There is nowhere to drop that.");
				return false;
			}
			dropper.ShowNearby(dropper, 
				$"You drop {dropping.GetString(Components.Visible, Text.FieldShortDesc)}.",
				$"{Text.Capitalize(dropper.GetString(Components.Visible, Text.FieldShortDesc))} drops {dropping.GetString(Components.Visible, Text.FieldShortDesc)}."
				);
			dropping.Move(dropInto);
			contents.Remove(dropping);
			return true;
		}
	}
}
