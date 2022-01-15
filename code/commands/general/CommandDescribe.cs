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
			if(cmd.RawInput.Length <= 0)
			{
				invoker.WriteLine("Please supply a new description to use.");
				return;
			}
			string lastDesc = invoker.GetValue<string>(Field.ExaminedDesc);
			invoker.SetValue<string>(Field.ExaminedDesc, cmd.RawInput);
			invoker.WriteLine($"Your appearance has been updated.\nFor reference, your last appearance was:\n{lastDesc}"); 
		}
	}
}