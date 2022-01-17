namespace inspiral
{
	internal class CommandUnequip : GameCommand
	{
		internal override void Initialize()
		{
			Aliases = new System.Collections.Generic.List<string>() { "unequip", "remove" };
			Description = "Unequips an object.";
			Usage = "unequip [object name or id]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent<InventoryComponent>())
			{
				invoker.WriteLine("You cannot unequip objects.");
				return;
			}
			if(invoker.TryUseBalance("poise"))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent<InventoryComponent>();
				if(inv.TryToUnequip(cmd.RawInput))
				{
					invoker.UseBalance("poise", 500);
				}
			}
		}
	}
}