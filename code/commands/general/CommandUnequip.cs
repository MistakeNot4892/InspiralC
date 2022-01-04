namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdUnequip(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent<InventoryComponent>())
			{
				invoker.WriteLine("You cannot unequip objects.");
				return;
			}
			if(invoker.TryUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent<InventoryComponent>();
				if(inv.TryToUnequip(cmd.rawInput))
				{
					invoker.UseBalance("poise", 500);
				}
				invoker.SendPrompt();
			}
		}
	}
}