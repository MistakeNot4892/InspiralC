namespace inspiral
{
	internal class CommandEquip : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "equip", "wear" };
			description = "Equips an object.";
			usage = "equip [object name or id] <slot>";
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
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent<InventoryComponent>();
				if(inv.TryToEquip(cmd.RawInput))
				{
					invoker.UseBalance("poise", 500);
				}
			}
		}
	}
}