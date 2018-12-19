using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdInfo(GameObject invoker, CommandData cmd)
		{
			if(invoker.location == null)
			{
				invoker.SendLine("You cannot see anything here.");
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
		}
	}
}