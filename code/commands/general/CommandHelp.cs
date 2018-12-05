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

		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.SendLineWithPrompt(invoker.GetClientSummary());
			return true;
		}
	}
}