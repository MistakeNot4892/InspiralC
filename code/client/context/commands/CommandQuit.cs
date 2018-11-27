using System.Collections.Generic;

namespace inspiral
{
	class CommandQuit : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "quit";
			aliases = new List<string>() {"quit", "qq"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.Quit();
			return true;
		}
	}
}