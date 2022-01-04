namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdTake(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent<InventoryComponent>())
			{
				invoker.WriteLine("You cannot hold objects.");
				invoker.SendPrompt();
				return;
			}
			if(invoker.TryUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent<InventoryComponent>();
				if(inv.TryToCollect(cmd.rawInput))
				{
					invoker.UseBalance("poise", 500);
				}
			}
			invoker.SendPrompt();
		}
	}
}