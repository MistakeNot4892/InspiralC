using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdUnequip(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent(Text.CompInventory))
			{
				invoker.WriteLine("You cannot unequip objects.");
				return;
			}
			if(invoker.CanUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent(Text.CompInventory);
				if(inv.TryToUnequip(cmd.rawInput))
				{
					invoker.UseBalance("poise", 500);
				}
				invoker.SendPrompt();
			}
		}
	}
}