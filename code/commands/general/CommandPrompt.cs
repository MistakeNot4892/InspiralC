namespace inspiral
{
	internal class CommandPrompt : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "prompt" };
			description = "Shows your full prompt.";
			usage = "prompt";
		}
		internal override void InvokeCommand(GameEntity invoker, CommandData cmd)
		{
			invoker.SendPrompt(true);
		}
	}
}