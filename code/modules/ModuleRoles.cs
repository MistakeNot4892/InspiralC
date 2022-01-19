using System.Collections.Generic;
namespace inspiral
{
	internal static partial class Modules
	{
		internal static RolesModule Roles { get { return (RolesModule)GetModule<RolesModule>(); } }
	}
	internal class RolesModule : GameModule
	{
		private Dictionary<string, GameRole> roles = new Dictionary<string, GameRole>();
		internal override void PostInitialize()
		{
			Game.LogError("Loading role definitions.");
			
			roles.Add("administrator", new GameRole(
				"Administrator", 
				"Contains role and game administration functions.", 
				new List<GameCommand>
				{
					Modules.Commands.GetCommand<CommandTest>(), 
					Modules.Commands.GetCommand<CommandViewRoles>(),
					Modules.Commands.GetCommand<CommandAddRole>(),
					Modules.Commands.GetCommand<CommandTakeRole>()
				}));

			roles.Add("builder", new GameRole(
				"Builder", 
				"Grants room and object viewing/modification functions.", 
				new List<GameCommand>
				{
					Modules.Commands.GetCommand<CommandCreate>(),
					Modules.Commands.GetCommand<CommandAddRoom>(),
					Modules.Commands.GetCommand<CommandSet>(),
					Modules.Commands.GetCommand<CommandView>()
				}));

			roles.Add("player", new GameRole(
				"Player", 
				"Grants general world interaction and communication commands.", 
				new List<GameCommand>
				{
					Modules.Commands.GetCommand<CommandSay>(),
					Modules.Commands.GetCommand<CommandEmote>(),
					Modules.Commands.GetCommand<CommandTake>(),
					Modules.Commands.GetCommand<CommandDrop>(),
					Modules.Commands.GetCommand<CommandInventory>(),
					Modules.Commands.GetCommand<CommandQuit>(),
					Modules.Commands.GetCommand<CommandLook>(),
					Modules.Commands.GetCommand<CommandExits>(),
					Modules.Commands.GetCommand<CommandClient>(),
					Modules.Commands.GetCommand<CommandHelp>(),
					Modules.Commands.GetCommand<CommandConfig>(),
					Modules.Commands.GetCommand<CommandEquip>(),
					Modules.Commands.GetCommand<CommandUnequip>(),
					Modules.Commands.GetCommand<CommandDescribe>(),
					Modules.Commands.GetCommand<CommandInfo>(),
					Modules.Commands.GetCommand<CommandPrompt>(),
					Modules.Commands.GetCommand<CommandStrike>()
				}));

			Game.LogError("Done.");
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