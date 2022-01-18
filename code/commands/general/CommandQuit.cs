namespace inspiral
{
	internal class CommandQuit : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("quit");
			Aliases.Add("qq");
			Description = "Quits the game, leaving your character asleep where they were.";
			Usage = "quit";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			invoker.Farewell("Goodbye!");
		}
	}
}