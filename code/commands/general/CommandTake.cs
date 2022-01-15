namespace inspiral
{
	internal class CommandTake : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "take", "get", "g", "pickup" };
			description = "Picks up an object.";
			usage = "take [object name or id]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent<InventoryComponent>())
			{
				invoker.WriteLine("You cannot hold objects.");
				return;
			}
			if(invoker.TryUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent<InventoryComponent>();
				if(inv.TryToCollect(cmd.RawInput))
				{
					invoker.UseBalance("poise", 500);
				}
			}
		}
	}
}