using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdQuit(GameClient invoker, string invocation)
		{
			invoker.SendLine("Goodbye!");
			invoker.Quit();
		}
	}
}