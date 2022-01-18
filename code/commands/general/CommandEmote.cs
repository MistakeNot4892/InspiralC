using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace inspiral
{
	internal class CommandEmote : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("emote");
			Aliases.Add("em");
			Aliases.Add("me");
			Description = "Performs a complex narration or action.";
			Usage = "emote <(preceeding text)> [following text]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			string emoteText = invoker.GetShortDesc();
			if(cmd.RawInput[0] == '(' && cmd.RawInput.IndexOf(')') != -1)
			{
				int secondParen = cmd.RawInput.IndexOf(')')-1;
				string rst = cmd.RawInput;
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
				emoteText = Text.FormatProse($"{emoteText} {cmd.RawInput}");
			}
			if(invoker.Location != null)
			{
				Dictionary<GameObject, string> showingMessages = new Dictionary<GameObject, string>();

				foreach(Match m in Text.mentionRegex.Matches(emoteText))
				{
					GameObject? mentioned = null;
					string findingRaw = m.Groups[1].Value.ToString();
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
					string pronounToken = m.Groups[2].Value.ToString().ToLower();

					if(pronounToken != null && pronounToken != "" && !Program.Game.Mods.Gender.Tokens.Contains(pronounToken))
					{
						invoker.WriteLine($"Unknown token '{pronounToken}'. Valid tokens for emotes are: {Text.EnglishList(Program.Game.Mods.Gender.Tokens)}."); 
						return;
					}
					if(finding != null && !showingMessages.ContainsKey(mentioned))
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
					foreach(KeyValuePair<GameObject, string> showing in showingMessages)
					{
						string finalMessage = emoteText;
						foreach(KeyValuePair<GameObject, string> subject in showingMessages)
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