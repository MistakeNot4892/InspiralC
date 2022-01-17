using System.Collections.Generic;
using System.Linq;

namespace inspiral
{

	internal static partial class Modules
	{
		internal static CommandModule Commands;
	}
	internal partial class CommandModule : GameModule
	{
		private Dictionary<System.Type, GameCommand> commands = new Dictionary<System.Type, GameCommand>();
		internal override void Initialize()
		{
			Modules.Commands = this;
			Game.LogError($"Building command dictionary.");
			foreach(var t in (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameCommand))
				select assemblyType))
			{
				Game.LogError($"- Creating command instance {t}.");
				GameCommand command = (GameCommand)System.Activator.CreateInstance(t);
				if(command != null && command.Aliases != null && command.Aliases.Count > 0)
				{
					RegisterCommand(command);
				}
			}
			Game.LogError($"Done.");
		}
		internal void RegisterCommand(GameCommand command)
		{
			commands.Add(command.GetType(), command);
		}
		internal GameCommand GetCommand(System.Type cmdType)
		{
			if(commands.ContainsKey(cmdType))
			{
				return commands[cmdType];
			}
			return null;

		}
		internal GameCommand GetCommand(string cmdClass)
		{
			System.Type cmdType = System.Type.GetType(cmdClass);
			if(cmdType == null)
			{
				Game.LogError($"Could not determine a valid command type from '{cmdClass}'.");
				return null;
			}
			return GetCommand(cmdType);
		}
		internal GameCommand GetCommand<T>()
		{
			return GetCommand(typeof(T));
		}
	}
}