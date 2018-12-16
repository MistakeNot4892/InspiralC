using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdPrompt(GameClient invoker, string invocation)
		{
			invoker.lastPrompt = null;
			invoker.SendPrompt();
		}
	}
}