using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdHelp(GameObject invoker, CommandData cmd)
		{
			if(cmd.rawInput != "" && cmd.rawInput != null)
			{
				invoker.WriteLine($"If the help system existed, you'd be searching for '{cmd.rawInput}'.");
			}
			else
			{
				invoker.WriteLine("Available commands:");
				foreach(GameRole role in invoker.GetAccount()?.roles)
				{
					invoker.WriteLine(role.GetHelp());
				}
				invoker.WriteLine("\nEnd of command list.");
			}
			invoker.SendPrompt();
		}
	}
}