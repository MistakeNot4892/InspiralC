namespace inspiral
{
	internal class CommandUnequip : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "unequip", "remove" };
			description = "Unequips an object.";
			usage = "unequip [object name or id]";
		}
		internal override void InvokeCommand(GameEntity invoker, CommandData cmd)
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
			}
		}
	}
}