using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdEquip(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Equipment))
			{
				invoker.SendLine("You cannot equip objects.");
				return;
			}
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				invoker.SendLine("What do you wish to equip?");
				return;
			}
			string tokenRaw = tokens[0];
			string token = tokenRaw.ToLower();
			GameObject equipping = invoker.shell.FindGameObjectInContents(token);
			if(equipping == null)
			{
				invoker.SendLine($"You cannot see '{tokenRaw}' anywhere.");
				return;
			}

			string slot = "default";
			if(tokens.Length >= 2)
			{
				if(tokens[1].ToLower() == "to" && tokens.Length >= 3)
				{
					slot = tokens[2].ToLower();
				}
				else
				{
					slot = tokens[1].ToLower();
				}
			}
			EquipmentComponent equipment = (EquipmentComponent)invoker.shell.GetComponent(Components.Equipment);
			equipment.Equip(invoker.shell, equipping, slot);
		}
	}
}