using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdTake(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent(Text.CompInventory))
			{
				invoker.WriteLine("You cannot hold objects.");
				return;
			}
			if(invoker.CanUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent(Text.CompInventory);
				if(inv.TryToCollect(cmd.rawInput))
				{
					invoker.UseBalance("poise", 500);
				}
				invoker.SendPrompt();
			}
		}
	}
}