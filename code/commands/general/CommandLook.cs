namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdLook(GameObject invoker, CommandData cmd)
		{
			if(invoker.location == null)
			{
				invoker.SendLine("You cannot see anything here.");
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
		}
	}
}