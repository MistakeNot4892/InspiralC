namespace inspiral
{
	internal class CommandDrop : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("drop");
			Description = "Drops an object.";
			Usage = "drop [object name or id]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent<InventoryComponent>())
			{
				invoker.WriteLine("You cannot drop objects.");
				return;
			}
			if(invoker.TryUseBalance("poise"))
			{
				var invComp = invoker.GetComponent<InventoryComponent>();
				if(invComp != null)
				{
					InventoryComponent inv = (InventoryComponent)invComp;
					inv.TryToDrop(cmd.RawInput);
				}
			}
		}
	}
}