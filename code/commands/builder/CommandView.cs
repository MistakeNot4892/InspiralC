using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdView(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				invoker.WriteLine("What do you wish to view?");
			}
			else
			{
				GameObject viewing = invoker.shell.FindGameObjectNearby(tokens[0].ToLower());
				if(viewing == null)
				{
					invoker.WriteLine($"Cannot find '{tokens[0].ToLower()}' here.");
				}
				else
				{
					invoker.WriteLine(viewing.GetStringSummary(invoker));
				}
			}
			invoker.SendPrompt();
		}
	}
}