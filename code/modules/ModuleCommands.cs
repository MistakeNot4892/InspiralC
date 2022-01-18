using System.Collections.Generic;
using System.Linq;

namespace inspiral
{

	internal partial class Modules
	{
		internal CommandModule Commands = new CommandModule();
	}
	internal partial class CommandModule : GameModule
	{
		private Dictionary<System.Type, GameCommand> commands = new Dictionary<System.Type, GameCommand>();
		internal override void Initialize()
		{
			Game.LogError($"Building command dictionary.");
			foreach(var t in (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameCommand))
				select assemblyType))
			{
				Game.LogError($"- Creating command instance {t}.");
				var command = System.Activator.CreateInstance(t);
				if(command != null)
				{
					GameCommand gameComm = (GameCommand)command;
					if(gameComm.Aliases != null && gameComm.Aliases.Count > 0)
					{
						RegisterCommand(gameComm);
					}
				}
			}
			Game.LogError($"Done.");
		}
		internal void RegisterCommand(GameCommand command)
		{
			commands.Add(command.GetType(), command);
		}
		internal GameCommand? GetCommand(System.Type cmdType)
		{
			if(commands.ContainsKey(cmdType))
			{
				return commands[cmdType];
			}
			return null;

		}
		internal GameCommand? GetCommand(string cmdClass)
		{
			System.Type? cmdType = System.Type.GetType(cmdClass);
			if(cmdType == null)
			{
				Game.LogError($"Could not determine a valid command type from '{cmdClass}'.");
				return null;
			}
			return GetCommand(cmdType);
		}
		internal GameCommand? GetCommand<T>()
		{
			return GetCommand(typeof(T));
		}
	}
}