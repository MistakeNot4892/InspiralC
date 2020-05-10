namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdTest(GameObject invoker, CommandData cmd) 
		{
			if(invoker.HasComponent(Text.CompMobile))
			{
				MobileComponent mob = (MobileComponent)invoker.GetComponent(Text.CompMobile);
				invoker.WriteLine($"Grasp:  {string.Join(", ", mob.graspers.ToArray())}");
				invoker.WriteLine($"Stance: {string.Join(", ", mob.stance.ToArray())}");
				invoker.WriteLine($"Strike: {string.Join(", ", mob.strikers.ToArray())}");
				invoker.WriteLine($"Equip:  {string.Join(", ", mob.equipmentSlots.ToArray())}");
				invoker.SendPrompt();
			}
			else
			{
				invoker.SendLine("You aren't a mob.");
			}
		}
	}
}