using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandTest Test = new CommandTest();
	}
	class CommandTest : GameCommand
	{
		internal override string Description { get; set; } = "Test command, please ignore.";
		internal override string Command { get; set; } = "test";
		internal override List<string> Aliases { get; set; } = new List<string>() { "test" };
		internal override string Usage { get; set; } = "test";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.SendLineWithPrompt("No test code here boss.");
			return true;
		}
	}
}