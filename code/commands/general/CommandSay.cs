namespace inspiral
{
	internal class CommandSay : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "say", "ask" };
			description = "Vocally communicates with the people and things around you.";
			usage = "say <(emote text)> [speech text] <emoticon>";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			GameObject target = null;
			string invocation = cmd.rawInput;
			if(invocation.Length >= 3 && invocation.Substring(0,3) == "to " && invoker.location != null)
			{
				invocation = invocation.Substring(3);
				string targetName = invocation.Substring(0, invocation.IndexOf(' '));
				if(invocation.Length >= targetName.Length+1)
				{
					invocation = invocation.Substring(targetName.Length+1);
					target = invoker.FindGameObjectNearby(targetName);
				}
				if(target == null)
				{
					invoker.SendLine($"You cannot see '{targetName}' here.");
					invoker.SendPrompt(); 
					return;
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
				invoker.ShowNearby(invoker, $"You open your mouth but say nothing.", $"{invoker.GetShort()} opens {invoker.gender.Their} mouth but says nothing.");
				invoker.SendPrompt();
				return;
			}
			string prefix1p = $"You {speechVerb1p}";
			string prefix3p = $"{invoker.GetShort()} {speechVerb3p}";

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
				string targetName = target.GetShort();
				invoker.ShowNearby(
					invoker, 
					target,
					$"{prefix1p} to {targetName}, \"{invocation}\"",
					$"{prefix3p} to you, \"{invocation}\"",
					$"{prefix3p} to {targetName}, \"{invocation}\"" 
				);
			}
			else
			{
				invoker.ShowNearby(invoker, $"{prefix1p}, \"{invocation}\"", $"{prefix3p} \"{invocation}\"");
			}
			invoker.SendPrompt();
		}
	}
}