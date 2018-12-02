using System;
using System.Collections.Generic;

namespace inspiral
{
	internal class GameRole
	{
		internal string name;
		internal Dictionary<string, GameCommand> commands = new Dictionary<string, GameCommand>();
		internal void AddCommand(GameCommand command)
		{
			commands.Add(command.commandString, command);
			foreach(string alias in command.aliases)
			{
				if(!commands.ContainsKey(alias))
				{
					commands.Add(alias, command);
				}
			}
		}
	}
}