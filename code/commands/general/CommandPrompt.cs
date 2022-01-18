namespace inspiral
{
	internal class CommandPrompt : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("prompt");
			Description = "Shows your full prompt.";
			Usage = "prompt";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			invoker.SendPrompt(true);
		}
	}
}