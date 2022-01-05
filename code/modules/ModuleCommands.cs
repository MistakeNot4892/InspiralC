using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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
				if(command != null && command.aliases != null && command.aliases.Count > 0)
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
		internal CommandData(GameCommand command, string aliasUsed, string input)
		{
			strCmd = aliasUsed;
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
							case "an":
							case "the":
							case "my":
								if(!command.skipArticles)
								{
									lastSubstring += $" {s}";
								}
								break;
							case "in":
							case "with":
							case "at":
							case "to":
							case "from":
								if(command.skipTokenQualifiers)
								{
									lastSubstring += $" {s}";
									break;
								}
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

	internal class GameCommand
	{
		internal List<string> aliases = null;
		internal string usage;
		internal string description;
		internal bool skipArticles = true;         // Parse 'a' 'an' and 'the' as part of substrings instead of skipping.
		internal bool skipTokenQualifiers = false; // Do not parse 'with' 'from' etc. as special tokens.
		internal GameCommand() { Initialize(); }
		internal virtual void Initialize() {}
		internal virtual void InvokeCommand(GameEntity invoker, CommandData cmd) {}

		internal string GetSummary()
		{
			return $"[{Text.EnglishList(aliases)}]";
		}
	}
}