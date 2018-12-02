using System.Collections.Generic;

namespace inspiral
{
	class CommandTest : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "test";
			aliases = new List<string>() {"test"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			Dictionary<string, List<string>> dialogue = new Dictionary<string, List<string>>();
			dialogue.Add("An Aggressive Navy SEAL", new List<string>());
			dialogue["An Aggressive Navy SEAL"].Add("As you browse the Internet, you are accosted by a loud and brutish miscreant.");
			dialogue["An Aggressive Navy SEAL"].Add($"\n{Colours.Fg("He shouts, \"", Colours.White)}{Colours.Fg("What the fuck did you just fucking say about me, you little bitch?", Colours.BoldWhite)}{Colours.Fg("\"", Colours.White)}");
			dialogue["An Aggressive Navy SEAL"].Add($"\n{Colours.Fg("What do you do?", Colours.BoldYellow)}");
			dialogue.Add("Options", new List<string>());
			dialogue["Options"].Add("1. Stand fast. You have done nothing wrong.");
			dialogue["Options"].Add("2. Shield yourself with seven proxies.");
			dialogue["Options"].Add("3. Purchase a dog.");
			invoker.WriteLinePrompted(Text.FormatBlock(dialogue, invoker.config.wrapwidth));
			return true;
		}
	}
}