using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace inspiral
{
	internal static partial class Text
	{
		internal const int NestedWrapwidthModifier = -13;
		internal static List<string> exits = new List<string>();
		internal static Dictionary<string, string> shortExits = new Dictionary<string, string>();
		internal static Dictionary<string, string> reversedExits = new Dictionary<string, string>();
		internal const string DefaultEnterMessage =        "$Short$ enters from the $dir$.";
		internal const string DefaultLeaveMessage =        "$Short$ leaves to the $dir$";
		internal const string DefaultDeathMessage =        "$Short$ lies here, dead.";
		internal static Dictionary<string, List<string>> speechVerbs = new Dictionary<string, List<string>>();
		internal static Regex stripRegex = new Regex($"{'\u001b'}\\[\\d+;*\\d*m", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		internal static Regex mentionRegex = new Regex(@"\$([0-9a-z]+)_*([a-zA-Z]*)\$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		static Text()
		{

			speechVerbs.Add(":)", new List<string>() {"smile and say", "smiles and says"});
			speechVerbs.Add(":(", new List<string>() {"frown and say", "frowns and says"});
			speechVerbs.Add("!", new List<string>()  {"shout", "shouts"});
			speechVerbs.Add("?", new List<string>()  {"ask", "asks"});

			exits.Add("north");
			exits.Add("south");
			exits.Add("east");
			exits.Add("west");
			exits.Add("northeast");
			exits.Add("southeast");
			exits.Add("northwest");
			exits.Add("southwest");
			exits.Add("up");
			exits.Add("down");
			exits.Add("in");
			exits.Add("out");
			shortExits.Add("n","north");
			shortExits.Add("s","south");
			shortExits.Add("e","east");
			shortExits.Add("w","west");
			shortExits.Add("ne","northeast");
			shortExits.Add("se","southeast");
			shortExits.Add("nw","northwest");
			shortExits.Add("sw","southwest");
			shortExits.Add("u","up");
			shortExits.Add("d","down");
			reversedExits.Add("south","north");
			reversedExits.Add("north","south");
			reversedExits.Add("west","east");
			reversedExits.Add("east","west");
			reversedExits.Add("southwest","northeast");
			reversedExits.Add("northwest","southeast");
			reversedExits.Add("southeast","northwest");
			reversedExits.Add("northeast","southwest");
			reversedExits.Add("down","up");
			reversedExits.Add("up","down");
		}
		internal static string Wrap(string input, int wrapwidth)
		{
			if(wrapwidth <= 0)
			{
				return input;
			}
			string result = "";
			int counter = 0;
			int lastEnding = -1;
			int i = 0;
			while(i < input.Length)
			{
				result += input[i];
				// Skip straight over colour codes.
				if(input[i] == '\u001b')
				{
					while(i < input.Length && input[i] != 'm')
					{
						i++;
						result += input[i];
					}
				}
				else
				{
					// Track valid splitting points for nicer formatting of the final wrapped lines.
					if(input[i] == ' ' || input[i] == '.' || input[i] == '?' || input[i] == ',' || input[i] == '-')
					{
						lastEnding = i;
						counter++;
					}
					// If we hit a newline we can count that as a wrap.
					else if(input[i] == '\n')
					{
						counter = 0;
						lastEnding = -1;
					}
					else
					{
						counter++;
					}
					// Every x characters, split with a newline.
					if(counter >= wrapwidth)
					{
						// Backtrack a little if we saw a decent split point earlier.
						if(lastEnding != -1 && i != lastEnding)
						{
							int skip = i - lastEnding;
							i = lastEnding;
							result = result.Substring(0, result.Length - skip);
						}
						lastEnding = -1;
						result += '\n';
						counter = 0;
					}
				}
				i++;
			}
			return result;
		}
		internal static string StripColours(string input)
		{
			int removed = 0;
			foreach (Match match in stripRegex.Matches(input))
			{
				string tmp = $"{input.Substring(0, match.Index-removed)}{input.Substring((match.Index-removed)+match.Length)}";
				input = tmp;
				removed += match.Length;
			}
			return input;
		}
		internal static string Capitalize(string input)
		{
			return $"{input.Substring(0,1).ToUpper()}{input.Substring(1)}";
		}
		internal static string FormatProse(string input)
		{
			char ending = input[input.Length-1];
			if(ending != '.' && ending != '!' && ending != '?')
			{
				input = $"{input}.";
			}
			return Capitalize(input);
		}
		internal static string FormatPopup(GameObject viewer, string header, string boxContents, int wrapwidth)
		{
			string sideBar = Colours.Fg("||", viewer.GetColour(Text.ColourDefaultFrameSecondary));
			string useHeader = Wrap(header, wrapwidth - 14).Split('\n')[0];
			int padNeeded = wrapwidth - (StripColours(useHeader).Length + 7);
			string result = $"{Colours.Fg($"[{new System.String('=', padNeeded/2)}\\", viewer.GetColour(Text.ColourDefaultFramePrimary))} {Colours.Fg(useHeader, viewer.GetColour(Text.ColourDefaultFrameHighlight))} {Colours.Fg($"/{new System.String('=', padNeeded-(padNeeded/2))}]",viewer.GetColour(Text.ColourDefaultFramePrimary))}";
			string emptyLine = $" {sideBar}{new System.String(' ', wrapwidth-7)}{sideBar}";
			result += $"\n{emptyLine}";
			string[] useContents = Wrap(boxContents, wrapwidth - 10).Split('\n');
			for(int i = 0;i < useContents.Length;i++)
			{
				string useLine = useContents[i];
				result += $"\n {sideBar}  {Colours.Fg(useLine, viewer.GetColour(Text.ColourDefaultHighlight))}{new System.String(' ', wrapwidth - StripColours(useLine).Length - 9)}{sideBar}";
			}
			result += $"\n{emptyLine}";
			result += $"\n{Colours.Fg($"[{new System.String('=', wrapwidth-3)}]", viewer.GetColour(Text.ColourDefaultFramePrimary))}";
			return result;
		}
		internal static string FormatBlock(GameObject viewer, Dictionary<string, List<string>> formatLines, int wrapwidth)
		{
			string sideBar = Colours.Fg("||", viewer.GetColour(Text.ColourDefaultFrameSecondary));
			string divider = Colours.Fg($"[{new System.String('=', wrapwidth-3)}]", viewer.GetColour(Text.ColourDefaultFramePrimary));
			string emptyLine = $" {sideBar}{new System.String(' ', wrapwidth-7)}{sideBar}";
			string result = "";
			foreach(KeyValuePair<string, List<string>> subSection in formatLines)
			{
				result += $"\n{divider}";
				string[] headerLines = Text.Wrap(subSection.Key, wrapwidth+Text.NestedWrapwidthModifier).Split('\n');
				for(int i = 0;i < headerLines.Length;i++)
				{
					string headerLine = headerLines[i];
					int padNeeded = wrapwidth - (StripColours(headerLine).Length+7);
					int padLeft = padNeeded/2;
					int padRight = padNeeded-padLeft;
					result += $"\n {sideBar}{new System.String(' ', padLeft)}{Colours.Fg(headerLine, viewer.GetColour(Text.ColourDefaultFrameHighlight))}{new System.String(' ', padRight)}{sideBar}";
				}
				result += $"\n{divider}";
				result += $"\n{emptyLine}";
				foreach(string line in subSection.Value)
				{
					string[] subLines = Wrap(line, wrapwidth + Text.NestedWrapwidthModifier).Split("\n");
					for(int i = 0;i<subLines.Length;i++)
					{
						string subLine = subLines[i];
						result += $"\n {sideBar}   {Colours.Fg(subLine, viewer.GetColour(Text.ColourDefaultHighlight))}{new System.String(' ', wrapwidth - StripColours(subLine).Length - 10)}{sideBar}";
					}
				}
				result += $"\n{emptyLine}";
			}
			result += $"\n{divider}";
			return result;
		}
		internal static string EnglishList(List<string> input)
		{
			string result = "";
			int i = 0;
			while(i < input.Count)
			{
				if(i == 0)
				{
					result = $"{input[i]}";
				}
				else if(i+1 == input.Count)
				{
					result = $"{result} and {input[i]}";
				}
				else
				{
					result = $"{result}, {input[i]}";
				}
				i++;
			}
			return result;
		}
		internal static string EnglishList<T>(Dictionary<string, T> input)
		{
			List<string> keys = new List<string>();
			foreach(KeyValuePair<string, T> entry in input)
			{
				keys.Add(entry.Key);
			}
			return EnglishList(keys);
		}

		internal static string GameObjListToEnglishList(List<GameObject> input)
		{
			List<string> ids = new List<string>();
			foreach(GameObject entry in input)
			{
				ids.Add($"{entry.name} (#{entry.GetLong(Field.Id)})");
			}
			return EnglishList(ids);
		}

		internal static string ReplacePronouns(GameObject other, string message, bool thirdPerson = false)
		{
			string token = other.name.ToLower();
			if(!message.ToLower().Contains($"${token}"))
			{
				token = $"{other.GetLong(Field.Id)}";
			}
			return ReplacePronouns(token, other, message, thirdPerson);
		}
		internal static string ReplacePronouns(string token, GameObject other, string message, bool thirdPerson = false)
		{
			System.Console.WriteLine(message);
			if(message.ToLower().Contains($"${token}"))
			{
				string replacementValue = thirdPerson ? other.gender.They : "you";
				message = Regex.Replace(
					message,
					$"\\${token}_(he|she|they|ey)\\$",
					replacementValue,
					RegexOptions.IgnoreCase
				);
				if(!message.ToLower().Contains($"${token}"))
				{
					return message;
				}

				replacementValue = thirdPerson ? other.gender.Their : "your";
				message = Regex.Replace(
					message,
					$"\\${token}_(his|her|their|eir)\\$",
					replacementValue,
					RegexOptions.IgnoreCase
				);
				if(!message.ToLower().Contains($"${token}"))
				{
					return message;
				}

				replacementValue = thirdPerson ? other.gender.Them : "you";
				message = Regex.Replace(
					message,
					$"\\${token}_(him|her|them|em)\\$",
					replacementValue,
					RegexOptions.IgnoreCase
				);
				if(!message.ToLower().Contains($"${token}"))
				{
					return message;
				}

				replacementValue = thirdPerson ? other.gender.Is : "are";
				message = Regex.Replace(
					message,
					$"\\${token}_(is|are)\\$",
					replacementValue,
					RegexOptions.IgnoreCase
				);
				if(!message.ToLower().Contains($"${token}"))
				{
					return message;
				}

				replacementValue = thirdPerson ? other.gender.Self : "self";
				message = Regex.Replace(
					message,
					$"\\${token}_sel(ves|f)\\$",
					replacementValue,
					RegexOptions.IgnoreCase
				);
				if(!message.ToLower().Contains($"${token}"))
				{
					return message;
				}

				replacementValue = thirdPerson ? $"{other.GetShortDesc()}'s" : "your";
				message = Regex.Replace(
					message,
					$"\\${token}'s\\$",
					replacementValue,
					RegexOptions.IgnoreCase
				);
				if(!message.ToLower().Contains($"${token}"))
				{
					return message;
				}

				replacementValue = thirdPerson ? $"{other.GetShortDesc()}" : "you";
				message = Regex.Replace(
					message,
					$"\\${token}\\$",
					replacementValue,
					RegexOptions.IgnoreCase
				);
			}
			return message;
		}
	}
}