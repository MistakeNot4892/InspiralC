using System.Collections.Generic;

namespace inspiral
{
	class CommandUnequip : GameCommand
	{
		internal override string Description { get; set; } = "unequips an object.";
		internal override string Command { get; set; } = "unequip";
		internal override List<string> Aliases { get; set; } = new List<string>() { "unequip", "unwield", "remove" };
		internal override string Usage { get; set; } = "unequip [object name or id]";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Equipment))
			{
				invoker.SendLine("You cannot unequip objects.");
				return true;
			}
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				invoker.SendLine("What do you wish to unequip?");
				return true;
			}
			string tokenRaw = tokens[0];
			string token = tokenRaw.ToLower();
			GameObject unequipping = invoker.shell.FindGameObjectInContents(token);
			if(unequipping == null)
			{
				invoker.SendLine($"You cannot see '{tokenRaw}' anywhere.");
				return true;
			}
			EquipmentComponent equipment = (EquipmentComponent)invoker.shell.GetComponent(Components.Equipment);
			equipment.Unequip(invoker.shell, unequipping);
			return true;
		}
	}
}