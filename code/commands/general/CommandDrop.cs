namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdDrop(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent(Text.CompInventory))
			{
				invoker.SendLine("You cannot drop objects.");
				return;
			}
			if(invoker.CanUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent(Text.CompInventory);
				inv.TryToDrop(cmd.rawInput);
			}
			invoker.SendPrompt();
		}
	}
}