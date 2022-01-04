namespace inspiral
{
	internal class CommandQuit : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "quit", "qq" };
			description = "Quits the game, leaving your character asleep where they were.";
			usage = "quit";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			invoker.SendLine("Goodbye!");
			invoker.Quit();
		}
	}
}