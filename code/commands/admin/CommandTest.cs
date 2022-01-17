namespace inspiral
{
	internal class CommandTest : GameCommand
	{
		internal override void Initialize()
		{
			Aliases = new System.Collections.Generic.List<string>() { "test" };
			Description = "Test command, please ignore.";
			Usage = "test";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(invoker.HasComponent<MobileComponent>())
			{
				MobileComponent mob = (MobileComponent)invoker.GetComponent<MobileComponent>();
				invoker.WriteLine($"Grasp:  {string.Join(", ", mob.graspers.ToArray())}");
				invoker.WriteLine($"Stance: {string.Join(", ", mob.stance.ToArray())}");
				invoker.WriteLine($"Strike: {string.Join(", ", mob.strikers.ToArray())}");
				invoker.WriteLine($"Equip:  {string.Join(", ", mob.equipmentSlots.ToArray())}");
			}
			else
			{
				invoker.WriteLine("You aren't a mob.");
			}
		}
	}
}