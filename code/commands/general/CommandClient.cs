using System.Collections.Generic;

namespace inspiral
{
	class CommandClient : GameCommand
	{
		internal override string Description { get; set; } = "Shows your client details.";
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