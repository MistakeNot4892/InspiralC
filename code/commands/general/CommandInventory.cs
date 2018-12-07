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

			string inventorySummary = "";
			List<GameObject> alreadyShown = new List<GameObject>();

			inventorySummary += "\n\nYou are wielding:";
			if(inv.equipped.Count > 0)
			{
				foreach(KeyValuePair<string, GameObject> gameObj in inv.wielded)
				{
					inventorySummary += $"\n- {gameObj.Value.GetString(Components.Visible, Text.FieldShortDesc)} ({gameObj.Value.name}#{gameObj.Value.id}) - in your {gameObj.Key}.";
					alreadyShown.Add(gameObj.Value);
				}
			}
			else
			{
				inventorySummary += "\n- nothing.";
			}

			inventorySummary += "\n\nYou have equipped:";
			if(inv.equipped.Count > 0)
			{
				foreach(KeyValuePair<string, GameObject> gameObj in inv.equipped)
				{
					if(!alreadyShown.Contains(gameObj.Value))
					{
						inventorySummary += $"\n- {gameObj.Value.GetString(Components.Visible, Text.FieldShortDesc)} ({gameObj.Value.name}#{gameObj.Value.id}) - on your {gameObj.Key}.";
						alreadyShown.Add(gameObj.Value);
					}
				}
			}
			else
			{
				inventorySummary += "\n- nothing.";
			}
			
			inventorySummary += "\n\nYou are carrying:";
			List<string> invCarrying = new List<string>();
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
		}
	}
}