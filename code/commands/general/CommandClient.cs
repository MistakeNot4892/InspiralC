using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static bool CmdClient(GameClient invoker, string invocation)
		{
			invoker.SendLineWithPrompt(invoker.GetClientSummary());
			return true;
		}
	}
}