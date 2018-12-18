using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdInfo(GameClient invoker, string invocation)
		{
			if(invoker.shell.location == null)
			{
				invoker.SendLine("You cannot see anything here.");
				return;
			}
			GameObject examining = null;
			string[] tokens = invocation.Split(" ");
			if(tokens.Length >= 1 && tokens[0] != "")
			{
				examining = invoker.shell.FindGameObjectNearby(tokens[0].ToLower());
			}
			else 
			{
				examining = invoker.shell.FindGameObjectNearby("here");
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