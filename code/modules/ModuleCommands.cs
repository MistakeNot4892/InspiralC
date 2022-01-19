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
			Program.Game.LogError($"Building command dictionary.");
			foreach(var t in (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameCommand))
				select assemblyType))
			{
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
			Program.Game.LogError($"Done.");
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
				Program.Game.LogError($"Could not determine a valid command type from '{cmdClass}'.");
			}
			return GetCommand(cmdType);
		}
		internal GameCommand GetCommand<T>()
		{
			return GetCommand(typeof(T));
		}
	}
}