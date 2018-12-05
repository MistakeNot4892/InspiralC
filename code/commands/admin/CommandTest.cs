using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandTest Test = new CommandTest();
	}
	class CommandTest : GameCommand
	{
		internal override string Command { get; set; } = "test";
		internal override List<string> Aliases { get; set; } = new List<string>() { "test" };
		internal override string Usage { get; set; } = "test";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string replaceWith = "FUCK";
			string replaceThing = $"\\${"token"}_(his|her|their|eir)\\$";

			invoker.SendLineWithPrompt($"esc: {replaceThing}");
			invoker.SendLineWithPrompt($"1:{invocation}:{Regex.Replace(invocation, replaceThing, replaceWith, RegexOptions.IgnoreCase)}");

			return true;
		}
	}
}