using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdHelp(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" " );
			if(invocation != null && invocation != "")
			{
				invoker.WriteLine($"If the help system existed, you'd be searching for '{invocation}'.");
			}
			else
			{
				invoker.WriteLine("Available commands:");
				foreach(GameRole role in invoker.account.roles)
				{
					invoker.WriteLine(role.GetHelp());
				}
				invoker.WriteLine("\nEnd of command list.");
			}
			invoker.SendPrompt();
		}
	}
}