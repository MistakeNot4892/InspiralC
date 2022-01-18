using System.Collections.Generic;

namespace inspiral
{
	internal class CommandInventory : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("inventory");
			Aliases.Add("inv");
			Aliases.Add("ii");
			Aliases.Add("i");
			Description = "Shows your inventory.";
			Usage = "inventory";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			var invComp = invoker.GetComponent<InventoryComponent>();
			if(invComp == null)
			{
				invoker.WriteLine("You cannot hold objects."); 
				return;
			}
			InventoryComponent inv = (InventoryComponent)invComp;

			string inventorySummary = "You are carrying:";
			if(inv.carrying.Count > 0)
			{
				foreach(KeyValuePair<string, GameObject> gameObj in inv.carrying)
				{
					inventorySummary += $"\n- {gameObj.Value.GetShortDesc()} ({gameObj.Value.GetValue<string>(Field.Name)}#{gameObj.Value.GetValue<long>(Field.Id)}) - {gameObj.Key}.";
				}
			}
			else
			{
				inventorySummary += "\n- nothing.";
			}
			invoker.WriteLine(inventorySummary); 
		}
	}
}