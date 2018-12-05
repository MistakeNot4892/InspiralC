using System.Collections.Generic;

namespace inspiral
{
	class GameCommand
	{
		internal virtual string Command { get; set; } = null;
		internal virtual List<string> Aliases { get; set; } = null;
		internal virtual string Usage { get; set; } = "No usage information supplied.";
		internal virtual string Description { get; set; } = "No description supplied.";
		internal virtual bool Invoke(GameClient invoker, string invocation) 
		{
			return false;
		}
		internal string GetSummary()
		{
			return $"{Command} [{Text.EnglishList(Aliases)}]";//: {Usage}";
		}
	}
}