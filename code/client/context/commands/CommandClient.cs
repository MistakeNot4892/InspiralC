using System.Collections.Generic;

namespace inspiral
{
	class CommandClient : GameCommand
	{
		internal override void Initialize() 
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