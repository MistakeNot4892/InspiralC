using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdInventory(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Inventory))
			{
				invoker.SendLine("You cannot hold objects.");
				return;
			}
			InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Components.Inventory);

			string inventorySummary = "You are carrying:";
			if(inv.carrying.Count > 0)
			{
				foreach(KeyValuePair<string, GameObject> gameObj in inv.carrying)
				{
					inventorySummary += $"\n- {gameObj.Value.GetString(Components.Visible, Text.FieldShortDesc)} ({gameObj.Value.name}#{gameObj.Value.id}) - {gameObj.Key}.";
				}
			}
			else
			{
				inventorySummary += "\n- nothing.";
			}
			invoker.SendLineWithPrompt(inventorySummary);
		}
	}
}