using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandQuit Quit = new CommandQuit();
	}
	class CommandQuit : GameCommand
	{
		internal override string Usage { get; set; } = "quit";
		internal CommandQuit() 
		{
			commandString = "quit";
			aliases = new List<string>() {"quit", "qq"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.SendLine("Goodbye!");
			invoker.Quit();
			return true;
		}
	}
}