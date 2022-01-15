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
			if(invoker.Location == null)
			{
				invoker.WriteLine("You cannot see anything here."); 
				return;
			}

			string examineKey = cmd.ObjAt;
			GameObject examining = null;
			if(examineKey == null)
			{
				examineKey = cmd.ObjTarget;
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
				invoker.WriteLine("You can see nothing here by that name.");
			} 
		}
	}
}