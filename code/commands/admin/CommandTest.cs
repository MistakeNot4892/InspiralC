namespace inspiral
{
	internal class CommandTest : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "test" };
			description = "Test command, please ignore.";
			usage = "test";
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
				invoker.SendLine("You aren't a mob.");
			}
		}
	}
}