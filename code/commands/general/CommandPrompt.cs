namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdPrompt(GameObject invoker, CommandData cmd)
		{
			invoker.SendPrompt(true);
		}
	}
}