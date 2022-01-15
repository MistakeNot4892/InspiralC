namespace inspiral
{
	internal class CommandHelp : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "help" };
			description = "Shows details on commands. WIP.";
			usage = "help <term>";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.RawInput != "" && cmd.RawInput != null)
			{
				invoker.WriteLine($"If the help system existed, you'd be searching for '{cmd.RawInput}'.");
			}
			else
			{
				invoker.WriteLine("Available commands:");
				foreach(GameRole role in invoker.GetAccount()?.roles)
				{
					invoker.WriteLine(role.GetHelp());
				}
				invoker.WriteLine("\nEnd of command list.");
			}
		}
	}
}