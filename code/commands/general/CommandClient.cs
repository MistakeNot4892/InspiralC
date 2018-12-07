using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdClient(GameClient invoker, string invocation)
		{
			invoker.SendLineWithPrompt(invoker.GetClientSummary());
		}
	}
}