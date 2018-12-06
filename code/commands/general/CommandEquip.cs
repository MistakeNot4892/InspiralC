using System.Collections.Generic;

namespace inspiral
{
	class CommandEquip : GameCommand
	{
		internal override string Description { get; set; } = "Equips an object.";
		internal override string Command { get; set; } = "equip";
		internal override List<string> Aliases { get; set; } = new List<string>() { "equip", "wield" };
		internal override string Usage { get; set; } = "wield [object name or id]";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Equipment))
			{
				invoker.SendLine("You cannot equip objects.");
				return true;
			}
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				invoker.SendLine("What do you wish to equip?");
				return true;
			}
			string tokenRaw = tokens[0];
			string token = tokenRaw.ToLower();
			GameObject equipping = invoker.shell.FindGameObjectInContents(token);
			if(equipping == null)
			{
				invoker.SendLine($"You cannot see '{tokenRaw}' anywhere.");
				return true;
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
			return true;
		}
	}
}