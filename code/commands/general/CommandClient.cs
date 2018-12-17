using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdClient(GameClient invoker, string invocation)
		{
			invoker.SendLine(invoker.GetClientSummary());
		}
	}
}