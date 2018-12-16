using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdDescribe(GameClient invoker, string invocation)
		{
			invocation = invocation.Trim();
			if(invocation.Length <= 0)
			{
				invoker.SendLine("Please supply a new description to use.");
				return;
			}
			string lastDesc = invoker.shell.GetString(Components.Visible, Text.FieldExaminedDesc);
			invoker.shell.SetString(Components.Visible, Text.FieldExaminedDesc, invocation);
			invoker.SendLine($"Your appearance has been updated.\nFor reference, your last appearance was:\n{lastDesc}");
		}
	}
}