using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal static partial class Modules
	{
		internal static RolesModule Roles;
	}
	internal class RolesModule : GameModule
	{
		private Dictionary<string, GameRole> roles = new Dictionary<string, GameRole>();
		internal override void PostInitialize()
		{
			Modules.Roles = this;
			Game.LogError("Loading role definitions.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data/definitions/roles", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Game.LogError($"- Loading role definition {f.File}.");
				try
				{
					JObject r = JObject.Parse(File.ReadAllText(f.File));
					string roleName = (string)r["name"];
					string roleDescription = (string)r["description"];
					List<GameCommand> roleCommands = new List<GameCommand>();
					foreach(string cmd in r["commands"].Select(t => (string)t).ToList())
					{
						GameCommand _cmd = Modules.Commands.GetCommand(cmd);
						if(_cmd != null)
						{
							if(!roleCommands.Contains(_cmd))
							{
								roleCommands.Add(_cmd);
							}
						}
					}
					GameRole role = new GameRole(roleName, roleDescription, roleCommands);
					roles.Add(role.name.ToLower(), role);
				}
				catch(System.Exception e)
				{
					Game.LogError($"Exception when loading role from file {f.File} - {e.Message}");
				}
			}
			Game.LogError("Done.");
		}
		internal GameRole GetRole(string key)
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
		internal string description = "No description supplied.";
		internal List<GameCommand> UniqueCommands = new List<GameCommand>();
		internal Dictionary<string, GameCommand> AllCommands = new Dictionary<string, GameCommand>();

		internal GameRole(string _name, string _description, List<GameCommand> _cmds)
		{
			name = _name;
			description = _description;
			UniqueCommands = _cmds;
			foreach(GameCommand command in UniqueCommands)
			{
				foreach(string alias in command.aliases)
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
			string result = $"{description}\n";
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
				result += $"\n   [{Text.EnglishList(command.aliases)}]:\n";
				result += $"\n     Usage: {command.usage}\n     {command.description}";
			}
			return result;
		}
	}
}