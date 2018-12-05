using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandRoles Roles = new CommandRoles();
	}

	class CommandRoles : GameCommand
	{
		internal CommandRoles() 
		{
			commandString = "roles";
			aliases = new List<string>() {"roles"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			return true;
		}
	}
}