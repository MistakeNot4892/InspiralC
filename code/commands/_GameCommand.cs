using System.Collections.Generic;

namespace inspiral
{

	internal class CommandData
	{
		internal string StrCmd =    null;
		internal string[] StrArgs = null;
		internal string ObjTarget = null;
		internal string ObjWith =   null;
		internal string ObjIn =     null;
		internal string ObjAt =     null;
		internal string ObjTo =     null;
		internal string ObjFrom =   null;
		internal string RawInput =  null;
		private void SaveSubstringAsToken(string token, string substring)
		{
			switch(token)
			{
				case "args":
					StrArgs = substring.Split(" ");
					break;
				case "at":
					ObjAt = substring;
					break;
				case "with":
					ObjWith = substring;
					break;
				case "in":
					ObjIn = substring;
					break;
				case "to":
					ObjTo = substring;
					break;
				case "from":
					ObjFrom = substring;
					break;
			}
		}
		internal string GetSummary()
		{
			string safeStrCmd =    StrCmd    ?? "null";
			string safeObjTarget = ObjTarget ?? "null";
			string safeObjWith =   ObjWith   ?? "null";
			string safeObjIn =     ObjIn     ?? "null";
			string safeObjAt =     ObjAt     ?? "null";
			string safeObjTo =     ObjTo     ?? "null";
			string safeObjFrom =   ObjFrom   ?? "null";
			string[] safeStrArgs = StrArgs   ?? new string[] {"null"};
			return $"cmd [{safeStrCmd}] target [{safeObjTarget}] args [{string.Join(", ", safeStrArgs)}] with [{safeObjWith}] in [{safeObjIn}] at [{safeObjAt}] to [{safeObjTo}] from [{safeObjFrom}] raw [{RawInput}]";
		}
		internal CommandData(GameCommand command, string aliasUsed, string input)
		{
			StrCmd = aliasUsed;
			RawInput = input;
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
					string lastToken = "args";
					foreach(string s in validTokens)
					{
						switch(s)
						{
							case "a":
							case "an":
							case "the":
							case "my":
								if(!command.SkipArticles)
								{
									lastSubstring += $" {s}";
								}
								break;
							case "in":
							case "with":
							case "at":
							case "to":
							case "from":
								if(command.SkipTokenQualifiers)
								{
									lastSubstring += $" {s}";
									break;
								}
								if(lastToken != null && lastSubstring != "")
								{
									SaveSubstringAsToken(lastToken, lastSubstring.Trim());
								}
								lastToken = s;
								lastSubstring = "";
								break;
							default:
								lastSubstring += $" {s}";
								break;
						}
					}
					if(lastToken != null && lastSubstring != "")
					{
						SaveSubstringAsToken(lastToken, lastSubstring.Trim());
					}
				if(StrArgs != null && StrArgs.Length > 0)
				{
					ObjTarget = StrArgs[0];
				}
			}
		}
	}

	internal class GameCommand
	{
		internal List<string> Aliases = null;
		internal string Usage;
		internal string Description;
		internal bool SkipArticles = true;         // Parse 'a' 'an' and 'the' as part of substrings instead of skipping.
		internal bool SkipTokenQualifiers = false; // Do not parse 'with' 'from' etc. as special tokens.
		internal GameCommand() { Initialize(); }
		internal virtual void Initialize() {}
		internal virtual void InvokeCommand(GameObject invoker, CommandData cmd) {}

		internal string GetSummary()
		{
			return $"[{Text.EnglishList(Aliases)}]";
		}
	}
}