using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdDrop(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Inventory))
			{
				invoker.SendLine("You cannot drop objects.");
				return;
			}
			if(invoker.shell.CanUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Components.Inventory);
				inv.TryToDrop(invocation);
			}
			invoker.SendPrompt();
		}
	}
}