using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdPrompt(GameClient invoker, string invocation)
		{
			invoker.lastPrompt = null;
			invoker.SendPrompt();
		}
	}
}