namespace inspiral
{
	internal class CommandLook : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "look", "l", "ql" };
			description = "Examines a creature, object or location.";
			usage = "look <at> <target>";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(invoker.location == null)
			{
				invoker.SendLine("You cannot see anything here.");
				invoker.SendPrompt(); 
				return;
			}

			string examineKey = cmd.objAt;
			GameObject examining = null;
			if(examineKey == null)
			{
				examineKey = cmd.objTarget;
			}
			if(examineKey == null)
			{
				examining = invoker.FindGameObjectNearby("here");
			}
			else
			{
				examining = invoker.FindGameObjectNearby(examineKey);
				if(examining == null)
				{
					examining = invoker.FindGameObjectInContents(examineKey);
				}
			}

			if(examining != null)
			{
				examining.ExaminedBy(invoker, false);
			}
			else
			{
				invoker.SendLine("You can see nothing here by that name.");
			}
			invoker.SendPrompt(); 
		}
	}
}