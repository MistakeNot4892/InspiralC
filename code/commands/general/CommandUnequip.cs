using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdUnequip(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Equipment))
			{
				invoker.SendLine("You cannot unequip objects.");
				return;
			}
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				invoker.SendLine("What do you wish to unequip?");
				return;
			}
			string tokenRaw = tokens[0];
			string token = tokenRaw.ToLower();
			GameObject unequipping = invoker.shell.FindGameObjectInContents(token);
			if(unequipping == null)
			{
				invoker.SendLine($"You cannot see '{tokenRaw}' anywhere.");
				return;
			}
			EquipmentComponent equipment = (EquipmentComponent)invoker.shell.GetComponent(Components.Equipment);
			equipment.Unequip(invoker.shell, unequipping);
		}
	}
}