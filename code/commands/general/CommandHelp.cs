using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdHelp(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" " );
			if(invocation != null && invocation != "")
			{
				invoker.SendLineWithPrompt($"If the help system existed, you'd be searching for '{invocation}'.");
			}
			else
			{
				invoker.WriteLine("Available commands:");
				foreach(GameRole role in invoker.account.roles)
				{
					invoker.WriteLine(role.GetHelp());
				}
				invoker.SendLineWithPrompt("\nEnd of command list.");
			}
		}
	}
}