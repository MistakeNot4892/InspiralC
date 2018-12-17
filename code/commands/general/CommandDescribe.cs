using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdDescribe(GameClient invoker, string invocation)
		{
			invocation = invocation.Trim();
			if(invocation.Length <= 0)
			{
				invoker.SendLine("Please supply a new description to use.");
				return;
			}
			string lastDesc = invoker.shell.GetString(Text.CompVisible, Text.FieldExaminedDesc);
			invoker.shell.SetString(Text.CompVisible, Text.FieldExaminedDesc, invocation);
			invoker.SendLine($"Your appearance has been updated.\nFor reference, your last appearance was:\n{lastDesc}");
		}
	}
}