using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace inspiral
{

	internal static partial class Command
	{
		private static Dictionary<string, GameCommand> commands = new Dictionary<string, GameCommand>();
		static Command()
		{
			Debug.WriteLine($"Building command method lookup table.");
			
			Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
			MethodInfo[] methodInfos = typeof(Command).GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
			for(int i = 0;i < methodInfos.Length;i++)
			{
				if(methodInfos[i].Name.Substring(0, 3) == "Cmd")
				{
					Console.WriteLine($"Caching MethodInfo for {methodInfos[i].Name}.");
					methods.Add(methodInfos[i].Name, methodInfos[i]);
				}
			}

			Debug.WriteLine($"Building command dictionary.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data\definitions\commands", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Debug.WriteLine($"Loading command definition {f.File}.");
				try
				{
					JObject r = JObject.Parse(File.ReadAllText(f.File));
					List<string> aliases = new List<string>();
					foreach(JValue token in r["aliases"].Children<JValue>())
					{
						aliases.Add(token.ToString());
					}
					MethodInfo method = methods[(string)r["method"]];
					GameCommand command = new GameCommand(
						(string)r["command"],
						aliases,
						(string)r["usage"],
						(string)r["description"],
						method
					);
					commands.Add(command.command, command);
				}
				catch(Exception e)
				{
					Debug.WriteLine($"Exception when loading command from file {f.File} - {e.Message}");
				}
			}
			Debug.WriteLine($"Done.");
		}
		internal static GameCommand Get(string command)
		{
			command = command.ToLower();
			if(commands.ContainsKey(command))
			{
				return commands[command];
			}
			return null;
		}
	}
	internal class GameCommand
	{
		internal string command;
		internal List<string> aliases;
		internal string usage;
		internal string description;
		internal MethodInfo invokedMethod;
		internal GameCommand(string _command, List<string> _aliases, string _usage, string _description, MethodInfo _method)
		{
			command = _command;
			aliases = _aliases;
			usage = _usage;
			description = _description;
			invokedMethod = _method;
		}
		internal string GetSummary()
		{
			return $"{command} [{Text.EnglishList(aliases)}]";
		}
	}
}