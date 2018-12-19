using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdQuit(GameObject invoker, CommandData cmd)
		{
			invoker.SendLine("Goodbye!");
			invoker.Quit();
		}
	}
}