using System.Collections.Generic;

namespace inspiral
{
	class CommandLook : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "look";
			aliases = new List<string>() {"look", "l", "ql"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			if(invoker.shell.location != null)
			{
				invoker.shell.location.ExaminedBy(invoker, true);
			}
			else
			{
				invoker.WriteLinePrompted("You cannot see anything here.");
			}
			return true;
		}
	}
}