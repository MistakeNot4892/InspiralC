namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdEquip(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent<InventoryComponent>())
			{
				invoker.WriteLine("You cannot equip objects.");
				return;
			}
			if(invoker.TryUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent<InventoryComponent>();
				if(inv.TryToEquip(cmd.rawInput))
				{
					invoker.UseBalance("poise", 500);
				}
			}
			invoker.SendPrompt();
		}
	}
}