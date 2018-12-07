using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdWield(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Inventory))
			{
				invoker.SendLine("You cannot wield objects.");
				return;
			}
			InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Components.Inventory);
			inv.TryToWield(invocation);
		}
	}
}