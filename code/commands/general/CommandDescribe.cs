using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdDescribe(GameObject invoker, CommandData cmd)
		{
			if(cmd.rawInput.Length <= 0)
			{
				invoker.SendLine("Please supply a new description to use.");
				return;
			}
			string lastDesc = invoker.GetString(Text.CompVisible, Text.FieldExaminedDesc);
			invoker.SetString(Text.CompVisible, Text.FieldExaminedDesc, cmd.rawInput);
			invoker.SendLine($"Your appearance has been updated.\nFor reference, your last appearance was:\n{lastDesc}");
		}
	}
}