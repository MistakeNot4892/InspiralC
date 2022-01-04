namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdEquip(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent<InventoryComponent>())
			{
				invoker.WriteLine("You are unable to equip objects.");
				invoker.SendPrompt(); 
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