using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdUnequip(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Text.CompInventory))
			{
				invoker.WriteLine("You cannot unequip objects.");
				return;
			}
			if(invoker.shell.CanUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Text.CompInventory);
				if(inv.TryToUnequip(invocation))
				{
					invoker.shell.UseBalance("poise", 500);
				}
				invoker.SendPrompt();
			}
		}
	}
}