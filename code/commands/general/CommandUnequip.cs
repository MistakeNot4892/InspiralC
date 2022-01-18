namespace inspiral
{
	internal class CommandUnequip : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("unequip");
			Aliases.Add("remove");
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
				var invComp = invoker.GetComponent<InventoryComponent>();
				if(invComp != null)
				{
					InventoryComponent inv = (InventoryComponent)invComp;
					if(inv.TryToUnequip(cmd.RawInput))
					{
						invoker.UseBalance("poise", 500);
					}
				}
			}
		}
	}
}