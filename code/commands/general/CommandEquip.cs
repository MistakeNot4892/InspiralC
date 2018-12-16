using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdEquip(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Inventory))
			{
				invoker.WriteLine("You cannot equip objects.");
				return;
			}
			if(invoker.shell.CanUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Components.Inventory);
				if(inv.TryToEquip(invocation))
				{
					invoker.shell.UseBalance("poise", 500);
				}
			}
			invoker.SendPrompt();
		}
	}
}