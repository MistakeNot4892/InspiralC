using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdTake(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Text.CompInventory))
			{
				invoker.WriteLine("You cannot hold objects.");
				return;
			}
			if(invoker.shell.CanUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Text.CompInventory);
				if(inv.TryToCollect(invocation))
				{
					invoker.shell.UseBalance("poise", 500);
				}
				invoker.SendPrompt();
			}
		}
	}
}