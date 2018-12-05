using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandClient Client  = new CommandClient();
	}
	class CommandClient : GameCommand
	{
		internal CommandClient() 
		{
			commandString = "client";
			aliases = new List<string>() {"client"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.SendLineWithPrompt(invoker.GetClientSummary());
			return true;
		}
	}
}