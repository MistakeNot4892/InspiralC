using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace inspiral
{

	static class Commands
	{
		private static Dictionary<string, GameCommand> commands = new Dictionary<string, GameCommand>();
		static Commands()
		{
			Debug.WriteLine($"Loading commands.");
			foreach(var t in (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameCommand))
				select assemblyType))
			{
				Debug.WriteLine($"Loading command {t}.");
				GameCommand command = (GameCommand)Activator.CreateInstance(t);
				commands.Add(command.Command, command);
			}
			Debug.WriteLine($"Done.");
		}
		internal static GameCommand GetCommand(string command)
		{
			command = command.ToLower();
			if(commands.ContainsKey(command))
			{
				return commands[command];
			}
			return null;
		}
	}
	class GameCommand
	{
		internal virtual string Command { get; set; } = null;
		internal virtual List<string> Aliases { get; set; } = null;
		internal virtual string Usage { get; set; } = "No usage information supplied.";
		internal virtual string Description { get; set; } = "No description supplied.";
		internal virtual bool Invoke(GameClient invoker, string invocation) 
		{
			return false;
		}
		internal string GetSummary()
		{
			return $"{Command} [{Text.EnglishList(Aliases)}]";//: {Usage}";
		}
	}
}