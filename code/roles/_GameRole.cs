using System;
using System.Collections.Generic;

namespace inspiral
{

	internal static partial class Roles
	{
		private static Dictionary<string, GameRole> roles = new Dictionary<string, GameRole>();
		static Roles()
		{
			foreach(GameRole role in new List<GameRole>() {Administrator, Builder, Player})
			{
				roles.Add(role.name.ToLower(), role);
			}
		}
		internal static GameRole GetRole(string key)
		{
			key = key.ToLower();
			if(roles.ContainsKey(key))
			{
				return roles[key];
			}
			return null;
		}
	}
	internal class GameRole
	{
		internal string name;
		internal virtual string Description { get; set; } = "No description supplied.";
		internal Dictionary<string, GameCommand> commands = new Dictionary<string, GameCommand>();
		internal string GetSummary() 
		{ 
			string result = $"{Description}\n";
			if(commands.Count <= 0)
			{
				return $"{result}\nThis role has no associated commands.";
			}
			result += "\nCommands:";
			List<GameCommand> uniqueCommands = new List<GameCommand>();
			foreach(KeyValuePair<string, GameCommand> command in commands)
			{
				if(!uniqueCommands.Contains(command.Value))
				{
					uniqueCommands.Add(command.Value);
				}
			}
			foreach(GameCommand command in uniqueCommands)
			{
				result += $"\n   {command.GetSummary()}";
			}
			return result;
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