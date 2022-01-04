namespace inspiral
{
	internal class CommandDescribe : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "describe", "desc" };
			description = "Sets your personal description.";
			usage = "describe [new description]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.rawInput.Length <= 0)
			{
				invoker.SendLine("Please supply a new description to use.");
				return;
			}
			string lastDesc = invoker.GetString<VisibleComponent>(Text.FieldExaminedDesc);
			invoker.SetString<VisibleComponent>(Text.FieldExaminedDesc, cmd.rawInput);
			invoker.SendLine($"Your appearance has been updated.\nFor reference, your last appearance was:\n{lastDesc}");
			invoker.SendPrompt(); 
		}
	}
}