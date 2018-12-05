using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandClient Client  = new CommandClient();
	}
	class CommandClient : GameCommand
	{
		internal override string Command { get; set; } = "client";
		internal override List<string> Aliases { get; set; } = new List<string>() { "client" };
		internal override string Usage { get; set; } = "client";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.SendLineWithPrompt(invoker.GetClientSummary());
			return true;
		}
	}
}