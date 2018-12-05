using System.Collections.Generic;

namespace inspiral
{
	class GameCommand
	{
		internal string commandString;
		internal List<string> aliases;
		internal virtual string Usage { get; set; } = "No usage information supplied.";
		internal virtual bool Invoke(GameClient invoker, string invocation) 
		{
			return false;
		}
		internal string GetSummary()
		{
			return $"{commandString} [{Text.EnglishList(aliases)}]";//: {Usage}";
		}
	}
}