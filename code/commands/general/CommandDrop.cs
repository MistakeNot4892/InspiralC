namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdDrop(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent<InventoryComponent>())
			{
				invoker.SendLine("You cannot drop objects.");
				return;
			}
			if(invoker.TryUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent<InventoryComponent>();
				inv.TryToDrop(cmd.rawInput);
			}
			invoker.SendPrompt();
		}
	}
}