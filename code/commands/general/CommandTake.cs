using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdTake(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Inventory))
			{
				invoker.SendLine("You cannot hold objects.");
				return;
			}
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				invoker.SendLine("What do you wish to take?");
				return;
			}
			string tokenRaw = tokens[0];
			string token = tokenRaw.ToLower();
			GameObject collecting = invoker.shell.FindGameObjectNearby(token);
			if(collecting == null)
			{
				invoker.SendLine($"You cannot see '{tokenRaw}' anywhere.");
				return;
			}
			if(collecting == invoker.shell)
			{
				invoker.SendLine($"Tragically, you cannot pick yourself up by your bootlaces.");
				return;
			}
			InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Components.Inventory);
			inv.Collect(invoker.shell, collecting);
		}
	}
}