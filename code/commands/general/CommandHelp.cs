using System.Collections.Generic;

namespace inspiral
{
	class CommandHelp : GameCommand
	{
		internal override string Command { get; set; } = "help";
		internal override List<string> Aliases { get; set; } = new List<string>() { "help", "?" };
		internal override string Usage { get; set; } = "help <term>";
		internal override string Description { get; set; } = "Shows details on commands. WIP.";

		internal override bool Invoke(GameClient invoker, string invocation)
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
			return true;
		}
	}
}