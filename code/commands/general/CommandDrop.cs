using System.Collections.Generic;

namespace inspiral
{
	class CommandDrop : GameCommand
	{
		internal override string Description { get; set; } = "Drops an object.";
		internal override string Command { get; set; } = "drop";
		internal override List<string> Aliases { get; set; } = new List<string>() { "drop" };
		internal override string Usage { get; set; } = "drop [object name or id]";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Inventory))
			{
				invoker.SendLine("You cannot drop objects.");
				return true;
			}
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				invoker.SendLine("What do you wish to drop?");
				return true;
			}
			string tokenRaw = tokens[0];
			string token = tokenRaw.ToLower();
			GameObject dropping = invoker.shell.FindGameObjectInContents(token);
			if(dropping == null)
			{
				invoker.SendLine($"You cannot see '{tokenRaw}' anywhere.");
				return true;
			}
			InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Components.Inventory);
			inv.Drop(invoker.shell, invoker.shell.location, dropping);
			return true;
		}
	}
}