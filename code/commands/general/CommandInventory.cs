using System.Collections.Generic;

namespace inspiral
{
	class CommandInventory : GameCommand
	{
		internal override string Description { get; set; } = "Shows your inventory.";
		internal override string Command { get; set; } = "inventory";
		internal override List<string> Aliases { get; set; } = new List<string>() { "inventory", "inv", "ii", "i" };
		internal override string Usage { get; set; } = "inventory";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Inventory))
			{
				invoker.SendLine("You cannot hold objects.");
				return true;
			}
			string inventorySummary = "";
			List<GameObject> alreadyShown = new List<GameObject>();
			List<string> invString = new List<string>();
			if(invoker.shell.HasComponent(Components.Equipment))
			{
				EquipmentComponent equip = (EquipmentComponent)invoker.shell.GetComponent(Components.Equipment);
				inventorySummary += "You have equipped:";
				if(equip.equipped.Count > 0)
				{
					foreach(KeyValuePair<string, GameObject> gameObj in equip.equipped)
					{
						inventorySummary += $"\n- {gameObj.Value.GetString(Components.Visible, Text.FieldShortDesc)} ({gameObj.Value.name}#{gameObj.Value.id}) - {gameObj.Key}.";
						alreadyShown.Add(gameObj.Value);
					}
				}
				else
				{
					inventorySummary += "\n- nothing.";
				}
				inventorySummary += "\n\n";
			}
			inventorySummary += "You are carrying:";
			List<string> invCarrying = new List<string>();
			InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Components.Inventory);
			if(inv.contents.Count > 0)
			{
				foreach(GameObject gameObj in inv.contents)
				{
					if(!alreadyShown.Contains(gameObj))
					{
						invCarrying.Add($"- {gameObj.GetString(Components.Visible, Text.FieldShortDesc)} ({gameObj.name}#{gameObj.id})");
					}
				}
			}
			if(invCarrying.Count > 0)
			{
				inventorySummary += $"\n{string.Join("\n", invCarrying.ToArray())}";
			}
			else
			{
				inventorySummary += "\n- nothing.";
			}
			invoker.SendLineWithPrompt(inventorySummary);
			return true;
		}
	}
}