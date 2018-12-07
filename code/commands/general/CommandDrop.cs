using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdDrop(GameClient invoker, string invocation)
		{
			if(!invoker.shell.HasComponent(Components.Inventory))
			{
				invoker.SendLine("You cannot drop objects.");
				return;
			}
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				invoker.SendLine("What do you wish to drop?");
				return;
			}
			string tokenRaw = tokens[0];
			string token = tokenRaw.ToLower();
			GameObject dropping = invoker.shell.FindGameObjectInContents(token);
			if(dropping == null)
			{
				invoker.SendLine($"You cannot see '{tokenRaw}' anywhere.");
				return;
			}
			EquipmentComponent equipment = (EquipmentComponent)invoker.shell.GetComponent(Components.Equipment);
			if(equipment == null || equipment.ForceUnequip(invoker.shell, dropping))
			{
				InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Components.Inventory);
				inv.Drop(invoker.shell, invoker.shell.location, dropping);
			}
		}
	}
}