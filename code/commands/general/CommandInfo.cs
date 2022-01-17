namespace inspiral
{
	internal class CommandInfo : GameCommand
	{
		internal override void Initialize()
		{
			Aliases = new System.Collections.Generic.List<string>() { "info", "ih", "p", "probe" };
			Description = "Examines a nearby object or room.";
			Usage = "info [object name or id]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(invoker.Location == null)
			{
				invoker.WriteLine("You cannot see anything here."); 
				return;
			}
			GameObject examining = null;
			if(cmd.ObjTarget != null)
			{
				examining = invoker.FindGameObjectNearby(cmd.ObjTarget);
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
				invoker.WriteLine("You can see nothing here by that name.");
			} 
		}
	}
}