using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdInfo(GameClient invoker, string invocation)
		{
			if(invoker.shell.location == null)
			{
				invoker.SendLineWithPrompt("You cannot see anything here.");
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
				invoker.SendLineWithPrompt("You can see nothing here by that name.");
			}
		}
	}
}