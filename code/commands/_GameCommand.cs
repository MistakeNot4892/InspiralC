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

	class GameCommandInput
	{
		internal string command;
		internal string[] tokens;
		internal string rawInput;
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
		/*
		internal virtual GameCommandInput Parse(string inputString)
		{
			GameCommandInput input = new GameCommandInput();
			string[] tokens = inputString.Split(" ");
			input.command = tokens[0].ToLower();
			if(tokens.Length > 1)
			{
				int copyTokens = tokens.Length-1;
				input.tokens = new string[copyTokens];
				for(int i = 0;i<copyTokens;i++)
				{
					input.tokens[i] = tokens[i+1];
				}
			}
			input.rawInput = inputString;
			return input;
		}
		*/
	}
}