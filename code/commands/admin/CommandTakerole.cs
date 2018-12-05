using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandTakerole Takerole = new CommandTakerole();
	}

	class CommandTakerole : GameCommand
	{
		internal CommandTakerole() 
		{
			commandString = "takerole";
			aliases = new List<string>() {"takerole"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			return true;
		}
	}
}