using System.Collections.Generic;

namespace inspiral
{
	class CommandQuit : GameCommand
	{
		internal override string Description { get; set; } = "Quits the game, leaving your character asleep where they were.";
		internal override string Command { get; set; } = "quit";
		internal override List<string> Aliases { get; set; } = new List<string>() { "quit", "qq" };
		internal override string Usage { get; set; } = "quit";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.SendLine("Goodbye!");
			invoker.Quit();
			return true;
		}
	}
}