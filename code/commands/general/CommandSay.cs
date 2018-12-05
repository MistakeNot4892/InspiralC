using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandSay Say = new CommandSay();
	}
	class CommandSay : GameCommand
	{
		internal CommandSay() 
		{
			commandString = "say";
			aliases = new List<string>() {"say", "'"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{

			GameObject target = null;
			if(invocation.Substring(0,3) == "to " && invoker.shell.location != null)
			{
				invocation = invocation.Substring(3);
				string targetName = invocation.Substring(0, invocation.IndexOf(' '));
				if(invocation.Length >= targetName.Length+1)
				{
					invocation = invocation.Substring(targetName.Length+1);
					target = invoker.shell.FindGameObjectNearby(targetName);
				}
				if(target == null)
				{
					invoker.SendLineWithPrompt($"You cannot see '{targetName}' here.");
					return true;
				}
			}

			string speechVerb1p = "say";
			string speechVerb3p = "says";
			if(invocation.Length >= 2)
			{
				// Check for emoticons, then simple endings like !, ? etc.
				string ending = invocation.Substring(invocation.Length-2);
				if(Text.speechVerbs.ContainsKey(ending))
				{
					invocation = invocation.Substring(0, invocation.Length-ending.Length).Trim();
					speechVerb1p = Text.speechVerbs[ending][0];
					speechVerb3p = Text.speechVerbs[ending][1];
				}
				else
				{
					ending = invocation.Substring(invocation.Length-1);
					if(Text.speechVerbs.ContainsKey(ending))
					{
						speechVerb1p = Text.speechVerbs[ending][0];
						speechVerb3p = Text.speechVerbs[ending][1];
					}
				}
			}

			if(invocation.Length <= 0)
			{
				invoker.shell.ShowNearby(invoker.shell, $"You open your mouth but say nothing.", $"{invoker.shell.GetString(Components.Visible, Text.FieldShortDesc)} opens {invoker.shell.gender.His} mouth but says nothing.");
				return true;
			}
			string prefix1p = $"You {speechVerb1p}";
			string prefix3p = $"{invoker.shell.GetString(Components.Visible, Text.FieldShortDesc)} {speechVerb3p}";

			if(invocation[0] == '(' && invocation.IndexOf(')') != -1)
			{
				int secondParen = invocation.IndexOf(')');
				string subSpeech = Text.Capitalize($"{invocation.Substring(1, secondParen-1)}");
				invocation = invocation.Substring(secondParen+2);
				prefix1p = $"{subSpeech}, you {speechVerb1p}";
				prefix3p = $"{subSpeech}, {prefix3p}";
			}

			invocation = Text.FormatProse(invocation);
			if(target != null)
			{
				string targetName = target.GetString(Components.Visible, Text.FieldShortDesc);
				invoker.shell.ShowNearby(invoker.shell, $"{prefix3p} to {targetName}, \"{invocation}\"", new List<GameObject>() {invoker.shell, target});
				invoker.shell.ShowMessage($"{prefix1p} to {targetName}, \"{invocation}\"");
				target.ShowMessage($"{prefix3p} to you, \"{invocation}\"");
			}
			else
			{
				invoker.shell.ShowNearby(invoker.shell, $"{prefix1p}, \"{invocation}\"", $"{prefix3p} \"{invocation}\"");
			}
			return true;
		}
	}
}