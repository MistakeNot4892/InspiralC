namespace inspiral
{
	internal class CommandEquip : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("equip");
			Aliases.Add("wear");
			Description = "Equips an object.";
			Usage = "equip [object name or id] <slot>";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(!invoker.HasComponent<InventoryComponent>())
			{
				invoker.WriteLine("You are unable to equip objects."); 
				return;
			}
			if(invoker.TryUseBalance("poise"))
			{
				var invComp = invoker.GetComponent<InventoryComponent>();
				if(invComp != null)
				{
					InventoryComponent inv = (InventoryComponent)invComp;
					if(inv.TryToEquip(cmd.RawInput))
					{
						invoker.UseBalance("poise", 500);
					}
				}
			}
		}
	}
}