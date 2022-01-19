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
			
			roles.Add("administrator", new GameRole(
				"Administrator", 
				"Contains role and game administration functions.", 
				new List<GameCommand>
				{
					Program.Game.Mods.Commands.GetCommand<CommandTest>(), 
					Program.Game.Mods.Commands.GetCommand<CommandViewRoles>(),
					Program.Game.Mods.Commands.GetCommand<CommandAddRole>(),
					Program.Game.Mods.Commands.GetCommand<CommandTakeRole>()
				}));

			roles.Add("builder", new GameRole(
				"Builder", 
				"Grants room and object viewing/modification functions.", 
				new List<GameCommand>
				{
					Program.Game.Mods.Commands.GetCommand<CommandCreate>(),
					Program.Game.Mods.Commands.GetCommand<CommandAddRoom>(),
					Program.Game.Mods.Commands.GetCommand<CommandSet>(),
					Program.Game.Mods.Commands.GetCommand<CommandView>()
				}));

			roles.Add("player", new GameRole(
				"Player", 
				"Grants general world interaction and communication commands.", 
				new List<GameCommand>
				{
					Program.Game.Mods.Commands.GetCommand<CommandSay>(),
					Program.Game.Mods.Commands.GetCommand<CommandEmote>(),
					Program.Game.Mods.Commands.GetCommand<CommandTake>(),
					Program.Game.Mods.Commands.GetCommand<CommandDrop>(),
					Program.Game.Mods.Commands.GetCommand<CommandInventory>(),
					Program.Game.Mods.Commands.GetCommand<CommandQuit>(),
					Program.Game.Mods.Commands.GetCommand<CommandLook>(),
					Program.Game.Mods.Commands.GetCommand<CommandExits>(),
					Program.Game.Mods.Commands.GetCommand<CommandClient>(),
					Program.Game.Mods.Commands.GetCommand<CommandHelp>(),
					Program.Game.Mods.Commands.GetCommand<CommandConfig>(),
					Program.Game.Mods.Commands.GetCommand<CommandEquip>(),
					Program.Game.Mods.Commands.GetCommand<CommandUnequip>(),
					Program.Game.Mods.Commands.GetCommand<CommandDescribe>(),
					Program.Game.Mods.Commands.GetCommand<CommandInfo>(),
					Program.Game.Mods.Commands.GetCommand<CommandPrompt>(),
					Program.Game.Mods.Commands.GetCommand<CommandStrike>()
				}));

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