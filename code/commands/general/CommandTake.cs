namespace inspiral
{
	internal class CommandTake : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("take");
			Aliases.Add("get");
			Aliases.Add("g");
			Aliases.Add("pickup");
			Description = "Picks up an object.";
			Usage = "take [object name or id]";
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
				var invComp = invoker.GetComponent<InventoryComponent>();
				if(invComp != null)
				{
					InventoryComponent inv = (InventoryComponent)invComp;
					if(inv.TryToCollect(cmd.RawInput))
					{
						invoker.UseBalance("poise", 500);
					}
				}
			}
		}
	}
}