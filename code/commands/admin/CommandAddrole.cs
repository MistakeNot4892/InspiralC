using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandAddrole Addrole = new CommandAddrole();
	}
	class CommandAddrole : GameCommand
	{
		internal CommandAddrole() 
		{
			commandString = "addrole";
			aliases = new List<string>() {"addrole"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			return true;
		}
	}
}