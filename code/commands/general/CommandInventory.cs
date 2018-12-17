using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdInventory(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Text.CompInventory))
			{
				invoker.SendLine("You cannot hold objects.");
				return;
			}
			InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Text.CompInventory);

			string inventorySummary = "You are carrying:";
			if(inv.carrying.Count > 0)
			{
				foreach(KeyValuePair<string, GameObject> gameObj in inv.carrying)
				{
					inventorySummary += $"\n- {gameObj.Value.GetString(Text.CompVisible, Text.FieldShortDesc)} ({gameObj.Value.name}#{gameObj.Value.id}) - {gameObj.Key}.";
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