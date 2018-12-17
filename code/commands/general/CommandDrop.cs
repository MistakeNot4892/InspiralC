using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdDrop(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Text.CompInventory))
			{
				invoker.SendLine("You cannot drop objects.");
				return;
			}
			if(invoker.shell.CanUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Text.CompInventory);
				inv.TryToDrop(invocation);
			}
			invoker.SendPrompt();
		}
	}
}