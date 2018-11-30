using System.Collections.Generic;

namespace inspiral
{
	class CommandPassword : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "password";
			aliases = new List<string>() {"password"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			return true;
		}
	}
}