using System.Collections.Generic;

namespace inspiral
{
	internal class CommandInventory : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "inventory", "inv", "ii", "i" };
			description = "Shows your inventory.";
			usage = "inventory";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent<InventoryComponent>())
			{
				invoker.SendLine("You cannot hold objects."); 
				return;
			}
			InventoryComponent inv = (InventoryComponent)invoker.GetComponent<InventoryComponent>();

			string inventorySummary = "You are carrying:";
			if(inv.carrying.Count > 0)
			{
				foreach(KeyValuePair<string, GameObject> gameObj in inv.carrying)
				{
					inventorySummary += $"\n- {gameObj.Value.GetShort()} ({gameObj.Value.name}#{gameObj.Value.id}) - {gameObj.Key}.";
				}
			}
			else
			{
				inventorySummary += "\n- nothing.";
			}
			invoker.SendLine(inventorySummary); 
		}
	}
}