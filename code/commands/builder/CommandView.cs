using System.Collections.Generic;

namespace inspiral
{
	class CommandView : GameCommand
	{
		internal override string Description { get; set; } = "Views the components and fields of an object.";
		internal override string Command { get; set; } = "view";
		internal override List<string> Aliases { get; set; } = new List<string>() { "view", "vv" };
		internal override string Usage { get; set; } = "view [object name or id]";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				invoker.SendLineWithPrompt("What do you wish to view?");
				return true;
			}
			GameObject viewing = invoker.shell.FindGameObjectNearby(tokens[0].ToLower());
			if(viewing == null)
			{
				invoker.SendLineWithPrompt($"Cannot find '{tokens[0].ToLower()}' here.");
				return true;
			}
			invoker.SendLineWithPrompt(viewing.GetStringSummary(invoker));
			return true;
		}
	}
}