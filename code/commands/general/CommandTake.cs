using System.Collections.Generic;

namespace inspiral
{
	class CommandTake : GameCommand
	{
		internal override string Description { get; set; } = "Picks up an object.";
		internal override string Command { get; set; } = "take";
		internal override List<string> Aliases { get; set; } = new List<string>() { "take", "get", "g", "pickup" };
		internal override string Usage { get; set; } = "take [object name or id]";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Inventory))
			{
				invoker.SendLine("You cannot hold objects.");
				return true;
			}
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				invoker.SendLine("What do you wish to take?");
				return true;
			}
			string tokenRaw = tokens[0];
			string token = tokenRaw.ToLower();
			GameObject collecting = invoker.shell.FindGameObjectNearby(token);
			if(collecting == null)
			{
				invoker.SendLine($"You cannot see '{tokenRaw}' anywhere.");
				return true;
			}
			if(collecting == invoker.shell)
			{
				invoker.SendLine($"Tragically, you cannot pick yourself up by your bootlaces.");
				return true;
			}
			InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Components.Inventory);
			inv.Collect(invoker.shell, collecting);
			return true;
		}
	}
}