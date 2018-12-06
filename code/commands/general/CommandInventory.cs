using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandInventory Inv  = new CommandInventory();
	}
	class CommandInventory : GameCommand
	{
		internal override string Description { get; set; } = "Shows your inventory.";
		internal override string Command { get; set; } = "inventory";
		internal override List<string> Aliases { get; set; } = new List<string>() { "inventory", "inv", "ii" };
		internal override string Usage { get; set; } = "inventory";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Inventory))
			{
				invoker.SendLine("You cannot hold objects.");
				return true;
			}
			List<string> invString = new List<string>();
			InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Components.Inventory);
			foreach(GameObject gameObj in inv.contents)
			{
				invString.Add($"{gameObj.GetString(Components.Visible, Text.FieldShortDesc)} ({gameObj.name}#{gameObj.id})");
			}
			if(invString.Count > 0)
			{
				invoker.SendLineWithPrompt($"You are holding: {Text.EnglishList(invString)}.");
			}
			else
			{
				invoker.SendLineWithPrompt("You are not holding anything.");
			}
			return true;
		}
	}
}