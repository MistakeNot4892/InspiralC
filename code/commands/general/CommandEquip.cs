namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdEquip(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent(Text.CompInventory))
			{
				invoker.WriteLine("You cannot equip objects.");
				return;
			}
			if(invoker.CanUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent(Text.CompInventory);
				if(inv.TryToEquip(cmd.rawInput))
				{
					invoker.UseBalance("poise", 500);
				}
			}
			invoker.SendPrompt();
		}
	}
}