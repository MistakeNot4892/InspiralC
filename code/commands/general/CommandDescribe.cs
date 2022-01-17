namespace inspiral
{
	internal class CommandDescribe : GameCommand
	{
		internal override void Initialize()
		{
			Aliases = new System.Collections.Generic.List<string>() { "describe", "desc" };
			Description = "Sets your personal description.";
			Usage = "describe [new description]";
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