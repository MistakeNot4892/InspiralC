using System.Collections.Generic;

namespace inspiral
{
	class GameContext
	{
		internal Dictionary<string, GameCommand> commands;
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

		internal virtual bool TakeInput(GameClient invoker, string command, string arguments)
		{
			return InvokeCommand(invoker, command, arguments);
		}
		internal virtual void Initialize() {}
		internal virtual void OnContextSet(GameClient viewer) {}
		internal virtual void OnContextUnset(GameClient viewer) {}
		internal virtual string GetPrompt(GameClient viewer) 
		{
			return "";
		}
	}
}