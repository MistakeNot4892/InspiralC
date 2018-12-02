using System.Collections.Generic;

namespace inspiral
{
	class CommandQuit : GameCommand
	{
		internal CommandQuit() 
		{
			commandString = "quit";
			aliases = new List<string>() {"quit", "qq"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.WriteLine("Goodbye!");
			invoker.Quit();
			return true;
		}
	}
}