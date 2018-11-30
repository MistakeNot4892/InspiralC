using System.Collections.Generic;

namespace inspiral
{
	class CommandColours : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "colours";
			aliases = new List<string>() {"colours"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			Colours.ShowTo(invoker);
			return true;
		}
	}
}