using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdQuit(GameClient invoker, string invocation)
		{
			invoker.SendLine("Goodbye!");
			invoker.Quit();
		}
	}
}