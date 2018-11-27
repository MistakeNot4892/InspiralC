using System.Collections.Generic;

namespace inspiral
{
	class GameContext
	{
		private Dictionary<string, GameCommand> commands;
		internal GameContext()
		{
			commands = new Dictionary<string, GameCommand>();
			Initialize();
		}
		internal void AddCommand(GameCommand command)
		{
			command.Initialize();
			commands.Add(command.commandString, command);
			foreach(string alias in command.aliases)
			{
				if(!commands.ContainsKey(alias))
				{
					commands.Add(alias, command);
				}
			}
		}

		internal bool InvokeCommand(GameClient client, string command, string commandString)
		{
			if(commands.ContainsKey(command))
			{
				GameCommand gameCommand = commands[command];
				return gameCommand.Invoke(client, commandString);
			}
			return false;
		}
		internal virtual void Initialize() {}
	}
}