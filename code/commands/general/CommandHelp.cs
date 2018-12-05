using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandHelp Help = new CommandHelp();
	}
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
					invoker.WriteLine($"\n{role.Name}:");
					foreach(GameCommand command in role.UniqueCommands)
					{
						invoker.WriteLine($"\n   {command.Command} [{Text.EnglishList(command.Aliases)}]:\n\n     Usage: {command.Usage}\n     {command.Description}");
					}
				}
				invoker.SendLineWithPrompt("\nEnd of command list.");
			}
			return true;
		}
	}
}