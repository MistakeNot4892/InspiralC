using System.Collections.Generic;

namespace inspiral
{
	class GameCommand
	{
		internal string commandString;
		internal List<string> aliases;
		internal virtual bool Invoke(GameClient invoker, string invocation) 
		{
			return false;
		}
	}
}