using System.Collections.Generic;

namespace inspiral
{
	class CommandClient : GameCommand
	{
		internal CommandClient() 
		{
			commandString = "client";
			aliases = new List<string>() {"client"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.WriteLinePrompted(invoker.GetClientSummary());
			return true;
		}
	}
}