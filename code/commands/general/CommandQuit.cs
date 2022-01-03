namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdQuit(GameObject invoker, CommandData cmd)
		{
			Game.Objects.SaveObject(invoker);
			invoker.SendLine("Goodbye!");
			invoker.Quit();
		}
	}
}