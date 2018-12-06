using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace inspiral
{
	class CommandEmote : GameCommand
	{
		internal override string Description { get; set; } = "Performs a complex narration or action.";
		internal override string Command { get; set; } = "emote";
		internal override List<string> Aliases { get; set; } = new List<string>() { "emote", "em", "me" };
		internal override string Usage { get; set; } = "emote <(preceeding text)> [following text]";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string emoteText = invoker.shell.GetString(Components.Visible, Text.FieldShortDesc);
			if(invocation[0] == '(' && invocation.IndexOf(')') != -1)
			{
				int secondParen = invocation.IndexOf(')')-1;
				emoteText = Text.FormatProse($"{invocation.Substring(1, secondParen)} {emoteText} {invocation.Substring(secondParen + emoteText.Length - 5)}");
			}
			else
			{
				emoteText = Text.FormatProse($"{emoteText} {invocation}");
			}
			if(invoker.shell.location != null)
			{
				Dictionary<GameObject, string> showingMessages = new Dictionary<GameObject, string>();

				foreach(Match m in Text.mentionRegex.Matches(emoteText))
				{
					GameObject mentioned = null;
					string findingRaw = m.Groups[1]?.Value.ToString();
					string finding = findingRaw.ToLower();
					if(finding != null)
					{
						mentioned = invoker.shell.FindGameObjectNearby(finding);
					}
					if(mentioned == null)
					{
						invoker.SendLineWithPrompt($"You cannot see '{findingRaw}' here.");
						return true;
					}
					string pronounToken = m.Groups[2]?.Value.ToString().ToLower();
					if(pronounToken != null && pronounToken != "" && !Gender.Tokens.Contains(pronounToken))
					{
						invoker.SendLineWithPrompt($"Unknown token '{pronounToken}'. Valid tokens for emotes are: {Text.EnglishList(Gender.Tokens)}.");
						return true;
					}
					if(!showingMessages.ContainsKey(mentioned))
					{
						showingMessages.Add(mentioned, finding);
					}
				}
				if(!showingMessages.ContainsKey(invoker.shell))
				{
					showingMessages.Add(invoker.shell, "me");
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
						if(invoker.shell == showing.Key)
						{
							finalMessage = $"You have emoted: {finalMessage}";
						}
						showing.Key.ShowMessage(finalMessage);
					}
					return true;
				}
			}
			invoker.shell.ShowMessage($"You have emoted: {emoteText}");
			return true;
		}
	}
}