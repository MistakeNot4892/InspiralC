using System.Collections.Generic;
using System.Linq;

namespace inspiral
{

	internal static partial class Modules
	{
		internal static CommandModule Commands { get { return (CommandModule)GetModule<CommandModule>(); } }
	}
	internal partial class CommandModule : GameModule
	{
		private Dictionary<System.Type, GameCommand> commands = new Dictionary<System.Type, GameCommand>();
		internal override void Initialize()
		{
			Game.LogError($"Building command dictionary.");
			foreach(GameCommand gameComm in Game.InstantiateSubclasses<GameCommand>())
			{
				if(gameComm.Aliases != null && gameComm.Aliases.Count > 0)
				{
					RegisterCommand(gameComm);
				}
			}
			Game.LogError($"Done.");
		}
		internal void RegisterCommand(GameCommand command)
		{
			commands.Add(command.GetType(), command);
		}
		internal GameCommand GetCommand(System.Type? cmdType)
		{
			if(cmdType != null && commands.ContainsKey(cmdType))
			{
				return commands[cmdType];
			}
			return new GameCommand();
		}
		internal GameCommand GetCommand(string cmdClass)
		{
			System.Type? cmdType = System.Type.GetType(cmdClass);
			if(cmdType == null)
			{
				Game.LogError($"Could not determine a valid command type from '{cmdClass}'.");
			}
			return GetCommand(cmdType);
		}
		internal GameCommand GetCommand<T>()
		{
			return GetCommand(typeof(T));
		}
	}
}