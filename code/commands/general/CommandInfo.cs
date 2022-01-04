namespace inspiral
{
	internal class CommandInfo : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "info", "p", "probe" };
			description = "Examines a nearby object or room.";
			usage = "info [object name or id]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(invoker.location == null)
			{
				invoker.SendLine("You cannot see anything here.");
				invoker.SendPrompt(); 
				return;
			}
			GameObject examining = null;
			if(cmd.objTarget != null)
			{
				examining = invoker.FindGameObjectNearby(cmd.objTarget);
			}
			else 
			{
				examining = invoker.FindGameObjectNearby("here");
			}
			if(examining != null)
			{
				examining.Probed(invoker);
			}
			else
			{
				invoker.SendLine("You can see nothing here by that name.");
			}
			invoker.SendPrompt(); 
		}
	}
}