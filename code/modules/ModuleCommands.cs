using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace inspiral
{

	internal static partial class Modules
	{
		internal static CommandModule Commands;
	}
	internal partial class CommandModule : GameModule
	{
		private Regex matchCmdWithIn = new Regex(@"([a-zA-Z0-9]+) with (.+) in (.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private Regex matchCmdInWith = new Regex(@"([a-zA-Z0-9]+) in (.+) with (.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private Regex matchCmdWith =   new Regex(@"([a-zA-Z0-9]+) with (.+)",         RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private Regex matchCmdIn =     new Regex(@"([a-zA-Z0-9]+) in (.+)",           RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private Dictionary<string, GameCommand> commands = new Dictionary<string, GameCommand>();
		internal override void Initialize()
		{
			Modules.Commands = this;
			Debug.WriteLine($"Building command method lookup table.");
			
			Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
			MethodInfo[] methodInfos = typeof(CommandModule).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
			for(int i = 0;i < methodInfos.Length;i++)
			{
				if(methodInfos[i].Name.Substring(0, 3) == "Cmd")
				{
					Console.WriteLine($"- Caching MethodInfo for {methodInfos[i].Name}.");
					methods.Add(methodInfos[i].Name, methodInfos[i]);
				}
			}

			Debug.WriteLine($"Building command dictionary.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data\definitions\commands", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Debug.WriteLine($"- Loading command definition {f.File}.");
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
		internal GameCommand Get(string command)
		{
			command = command.ToLower();
			if(commands.ContainsKey(command))
			{
				return commands[command];
			}
			return null;
		}
		internal Tuple<string, string, string> FindTokensFromInput(string invocation)
		{
			Match match = matchCmdInWith.Match(invocation);
			if(match.Success)
			{
				return new Tuple<string, string, string>(
					StripTokenPrefixes(match.Groups[1].Value), 
					StripTokenPrefixes(match.Groups[2].Value), 
					StripTokenPrefixes(match.Groups[3].Value)
				);
			}
			match = matchCmdWithIn.Match(invocation);
			if(match.Success)
			{
				return new Tuple<string, string, string>(
					StripTokenPrefixes(match.Groups[1].Value), 
					StripTokenPrefixes(match.Groups[3].Value), 
					StripTokenPrefixes(match.Groups[2].Value)
				);
			}
			match = matchCmdIn.Match(invocation);
			if(match.Success)
			{
				return new Tuple<string, string, string>(
					StripTokenPrefixes(match.Groups[1].Value), 
					StripTokenPrefixes(match.Groups[2].Value), 
					null
				);
			}
			match = matchCmdWith.Match(invocation);
			if(match.Success)
			{
				return new Tuple<string, string, string>(
					StripTokenPrefixes(match.Groups[1].Value), 
					null, 
					StripTokenPrefixes(match.Groups[2].Value)
				);
			}
			return new Tuple<string, string, string>(
				StripTokenPrefixes(invocation), 
				null, 
				null
			);
		}
		private string StripTokenPrefixes(string token)
		{
			token = token.Trim().ToLower();
			if(token.Length >= 4 && token.Substring(0, 4) == "the ")
			{
				token = token.Substring(4);
			}
			if(token.Length >= 2 && token.Substring(0, 2) == "a ")
			{
				token = token.Substring(2);
			}
			return token; 
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