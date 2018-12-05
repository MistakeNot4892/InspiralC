using System;
using System.Collections.Generic;

namespace inspiral
{
	class GameContext
	{
		internal Dictionary<string, GameCommand> commands = new Dictionary<string, GameCommand>();
		internal virtual bool TakeInput(GameClient invoker, string command, string rawCommand, string arguments) { return false; }
		internal virtual void OnContextSet(GameClient viewer) {}
		internal virtual void OnContextUnset(GameClient viewer) {}
		internal virtual bool InvokeCommand(GameClient invoker, string command, string arguments) 
		{
			if(commands.ContainsKey(command))
			{
				GameCommand contextCommand = commands[command];
				return contextCommand.Invoke(invoker, arguments);
			}
			foreach(GameRole role in invoker.account.roles)
			{
				if(role.commands.ContainsKey(command))
				{
					GameCommand gameCommand = role.commands[command];
					return gameCommand.Invoke(invoker, arguments);
				}
			}
			return false;
		}
		internal virtual string GetPrompt(GameClient viewer) 
		{
			return "";
		}
		internal void AddCommand(GameCommand command)
		{
			commands.Add(command.Command, command);
			foreach(string alias in command.Aliases)
			{
				if(!commands.ContainsKey(alias))
				{
					commands.Add(alias, command);
				}
			}
		}
	}
}