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
				roles.Add(role.Name.ToLower(), role);
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
		internal Dictionary<string, GameCommand> AllCommands { get; set; } = new Dictionary<string, GameCommand>();
		internal virtual string Name { get; set; } = "unnamed";
		internal virtual string Description { get; set; } = "No description supplied.";
		internal virtual List<GameCommand> UniqueCommands { get; set; } = new List<GameCommand>();

		internal GameRole()
		{
			foreach(GameCommand command in UniqueCommands)
			{
				if(!AllCommands.ContainsKey(command.Command))
				{
					AllCommands.Add(command.Command, command);
				}
			}
		}

		internal string GetSummary() 
		{ 
			string result = $"{Description}\n";
			if(AllCommands.Count <= 0)
			{
				return $"{result}\nThis role has no associated commands.";
			}
			result += "\nCommands:";
			foreach(GameCommand command in UniqueCommands)
			{
				result += $"\n   {command.GetSummary()}";
			}
			return result;
		}
	}
}