using System.Collections.Generic;

namespace inspiral
{
	class CommandConfig : GameCommand
	{
		internal override string Description { get; set; } = "Configures various options related to gameplay and presentation. WIP.";
		internal override string Command { get; set; } = "config";
		internal override List<string> Aliases { get; set; } = new List<string>() { "config" };
		internal override string Usage { get; set; } = "config [option] <value>";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.SendLineWithPrompt("Config not implemented yet sorry.");
			return true;
		}
	}
}