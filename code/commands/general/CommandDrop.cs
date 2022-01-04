namespace inspiral
{
	internal class CommandDrop : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "drop" };
			description = "Drops an object.";
			usage = "drop [object name or id]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
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