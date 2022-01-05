using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace inspiral
{
	internal class CommandEmote : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "emote", "em", "me" };
			description = "Performs a complex narration or action.";
			usage = "emote <(preceeding text)> [following text]";
		}
/*
{
}
*/
		internal override void InvokeCommand(GameEntity invoker, CommandData cmd)
		{
			string emoteText = invoker.GetShort();
			if(cmd.rawInput[0] == '(' && cmd.rawInput.IndexOf(')') != -1)
			{
				int secondParen = cmd.rawInput.IndexOf(')')-1;
				string rst = cmd.rawInput;
				if(rst.Length <= secondParen+3)
				{
					invoker.WriteLine("Please specify emote text after the preface."); 
					return;
				}
				string end = rst.Substring(secondParen + 3);
				emoteText = Text.FormatProse($"{rst.Substring(1, secondParen)} {emoteText} {end}");
			}
			else
			{
				emoteText = Text.FormatProse($"{emoteText} {cmd.rawInput}");
			}
			if(invoker.location != null)
			{
				Dictionary<GameEntity, string> showingMessages = new Dictionary<GameEntity, string>();

				foreach(Match m in Text.mentionRegex.Matches(emoteText))
				{
					GameEntity mentioned = null;
					string findingRaw = m.Groups[1]?.Value.ToString();
					string finding = findingRaw.ToLower();
					if(finding != null)
					{
						mentioned = invoker.FindGameObjectNearby(finding);
					}
					if(mentioned == null)
					{
						invoker.WriteLine($"You cannot see '{findingRaw}' here."); 
						return;
					}
					string pronounToken = m.Groups[2]?.Value.ToString().ToLower();
					if(pronounToken != null && pronounToken != "" && !Modules.Gender.Tokens.Contains(pronounToken))
					{
						invoker.WriteLine($"Unknown token '{pronounToken}'. Valid tokens for emotes are: {Text.EnglishList(Modules.Gender.Tokens)}."); 
						return;
					}
					if(!showingMessages.ContainsKey(mentioned))
					{
						showingMessages.Add(mentioned, finding);
					}
				}
				if(!showingMessages.ContainsKey(invoker))
				{
					showingMessages.Add(invoker, "me");
				}
				if(showingMessages.Count >= 1)
				{
					foreach(KeyValuePair<GameEntity, string> showing in showingMessages)
					{
						string finalMessage = emoteText;
						foreach(KeyValuePair<GameEntity, string> subject in showingMessages)
						{
							finalMessage = Text.ReplacePronouns(subject.Value, subject.Key, finalMessage, (subject.Key != showing.Key));
						}
						if(invoker == showing.Key)
						{
							finalMessage = $"You have emoted: {finalMessage}";
						}
						showing.Key.WriteLine(finalMessage, true);
					}
					return;
				}
			}
			invoker.WriteLine($"You have emoted: {emoteText}", true); 
		}
	}
}