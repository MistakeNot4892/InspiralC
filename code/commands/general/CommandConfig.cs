using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static bool CmdConfig(GameClient invoker, string invocation)
		{
			invoker.SendLineWithPrompt("Config not implemented yet sorry.");
			return true;
		}
	}
}