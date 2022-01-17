namespace inspiral
{
	internal class CommandPrompt : GameCommand
	{
		internal override void Initialize()
		{
			Aliases = new System.Collections.Generic.List<string>() { "prompt" };
			Description = "Shows your full prompt.";
			Usage = "prompt";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			invoker.SendPrompt(true);
		}
	}
}