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
			foreach (var f in (from file in Directory.EnumerateFiles(@"data/definitions/commands", "*.json", SearchOption.AllDirectories) select new { File = file }))
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

		internal class CommandData
		{
			internal string strCmd =    null;
			internal string[] strArgs = null;
			internal string objTarget = null;
			internal string objWith =   null;
			internal string objIn =     null;
			internal string objAt =     null;
			internal string objTo =     null;
			internal string objFrom =   null;
			internal string rawInput =  null;
			private void SaveSubstringAsField(string field, string substring)
			{
				switch(field)
				{
					case "args":
						strArgs = substring.Split(" ");
						break;
					case "at":
						objAt = substring;
						break;
					case "with":
						objWith = substring;
						break;
					case "in":
						objIn = substring;
						break;
					case "to":
						objTo = substring;
						break;
					case "from":
						objFrom = substring;
						break;
				}
			}
			internal string GetSummary()
			{
				string safeStrCmd =    strCmd ==    null ? "null" :                strCmd;
				string safeObjTarget = objTarget == null ? "null" :                objTarget;
				string[] safeStrArgs = strArgs ==   null ? new string[] {"null"} : strArgs;
				string safeObjWith =   objWith ==   null ? "null" :                objWith;
				string safeObjIn =     objIn ==     null ? "null" :                objIn;
				string safeObjAt =     objAt ==     null ? "null" :                objAt;
				string safeObjTo =     objTo ==     null ? "null" :                objTo;
				string safeObjFrom =   objFrom ==   null ? "null" :                objFrom;

				return $"cmd [{safeStrCmd}] target [{safeObjTarget}] args [{string.Join(", ", safeStrArgs)}] with [{safeObjWith}] in [{safeObjIn}] at [{safeObjAt}] to [{safeObjTo}] from [{safeObjFrom}] raw [{rawInput}]";
			}
			internal CommandData(string command, string input)
			{
				strCmd = command;
				rawInput = input;
				string[] tokens = input.ToLower().Split(" ");
				List<string> validTokens = new List<string>();
				for(int i = 0;i<tokens.Length;i++)
				{
					string t = tokens[i].Trim();
					if(t != "")
					{
						validTokens.Add(t);
					}
				}
				if(validTokens.Count > 0)
				{
					string lastSubstring = "";
					string lastField = "args";
					foreach(string s in validTokens)
					{
						switch(s)
						{
							case "a":
							case "the":
								break;
							case "in":
							case "with":
							case "at":
							case "to":
							case "from":
								if(lastField != null && lastSubstring != "")
								{
									SaveSubstringAsField(lastField, lastSubstring.Trim());
								}
								lastField = s;
								lastSubstring = "";
								break;
							default:
								lastSubstring += $" {s}";
								break;
						}
					}
					if(lastField != null && lastSubstring != "")
					{
						SaveSubstringAsField(lastField, lastSubstring.Trim());
					}
					if(strArgs != null && strArgs.Length > 0)
					{
						objTarget = strArgs[0];
					}
				}
			}
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