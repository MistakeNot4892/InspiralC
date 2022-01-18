using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class Modules
	{
		internal RolesModule Roles = new RolesModule();
	}
	internal class RolesModule : GameModule
	{
		private Dictionary<string, GameRole> roles = new Dictionary<string, GameRole>();
		internal override void PostInitialize()
		{
			Program.Game.LogError("Loading role definitions.");
			// TODO readd role repo.
			Program.Game.LogError("Done.");
		}
		internal GameRole? GetRole(string key)
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
		internal string name = "unnamed";
		internal string Description = "No description supplied.";
		internal List<GameCommand> UniqueCommands = new List<GameCommand>();
		internal Dictionary<string, GameCommand> AllCommands = new Dictionary<string, GameCommand>();

		internal GameRole(string _name, string _description, List<GameCommand> _cmds)
		{
			name = _name;
			Description = _description;
			UniqueCommands = _cmds;
			foreach(GameCommand command in UniqueCommands)
			{
				foreach(string alias in command.Aliases)
				{
					if(!AllCommands.ContainsKey(alias))
					{
						AllCommands.Add(alias, command);
					}
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
		internal string GetHelp()
		{
			string result = $"{name}:";
			foreach(GameCommand command in UniqueCommands)
			{
				result += $"\n   [{Text.EnglishList(command.Aliases)}]:\n";
				result += $"\n     Usage: {command.Usage}\n     {command.Description}";
			}
			return result;
		}
	}
}