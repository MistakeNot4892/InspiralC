using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace inspiral
{
	internal static class Text
	{
		internal const int FieldShortDesc =    0;
		internal const int FieldRoomDesc =     1;
		internal const int FieldExaminedDesc = 2;
		internal const int FieldEnterMessage = 3;
		internal const int FieldLeaveMessage = 4;
		internal const int FieldDeathMessage = 5;

		internal static List<string> exits = new List<string>();
		internal static Dictionary<string, string> shortExits = new Dictionary<string, string>();
		internal static Dictionary<string, string> reversedExits = new Dictionary<string, string>();

		internal const string DefaultRoomShort =           "an empty room";
		internal const string DefaultRoomLong =            "This is a completely empty room.";
		internal const string DefaultName =                "object";
		internal const string DefaultShortDescription =    "a generic object";
		internal const string DefaultRoomDescription =     "A generic object is here.";
		internal const string DefaultExaminedDescription = "This is a generic object. Fascinating stuff.";
		internal const string DefaultEnterMessage =        "A generic object enters from the $DIR.";
		internal const string DefaultLeaveMessage =        "A generic object leaves to the $DIR";
		internal const string DefaultDeathMessage =        "A generic object lies here, dead.";
		internal static string stripRegexPattern = $"{'\u001b'}\\[\\d+;*\\d*m";
		internal static Regex stripRegex = new Regex(stripRegexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
		static Text()
		{
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
					// Trailing spaces at the start of a line are annoying.
					/*
					else if(input[i] == ' ' && counter == 1)
					{
						result = result.Substring(0, result.Length-1);
					}
					*/
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
		
		private static string sideBar = Colours.Fg("||", Colours.Blue);

		internal static string FormatPopup(string header, string boxContents, int wrapwidth)
		{

			string useHeader = Wrap(header, wrapwidth - 14).Split('\n')[0];
			int padNeeded = wrapwidth - (StripColours(useHeader).Length + 7);
			string result = $"{Colours.Fg($"[{new String('=', padNeeded/2)}\\", Colours.Cyan)} {Colours.Fg(useHeader, Colours.BoldCyan)} {Colours.Fg($"/{new String('=', padNeeded-(padNeeded/2))}]",Colours.Cyan)}";

			string emptyLine = $" {sideBar}{new String(' ', wrapwidth-7)}{sideBar}";
			result += $"\n{emptyLine}";

			string[] useContents = Wrap(boxContents, wrapwidth - 10).Split('\n');
			for(int i = 0;i < useContents.Length;i++)
			{
				string useLine = useContents[i];
				result += $"\n {sideBar}  {Colours.Fg(useLine, Colours.BoldWhite)}{new String(' ', wrapwidth - StripColours(useLine).Length - 9)}{sideBar}";
			}

			result += $"\n{emptyLine}";
			result += $"\n{Colours.Fg($"[{new String('=', wrapwidth-3)}]", Colours.Cyan)}";
			return result;
		}

		internal static string FormatBlock(Dictionary<string, List<string>> formatLines, int wrapwidth)
		{

			string divider = Colours.Fg($"[{new String('=', wrapwidth-3)}]", Colours.Cyan);
			string emptyLine = $" {sideBar}{new String(' ', wrapwidth-7)}{sideBar}";
			string result = "";

			foreach(KeyValuePair<string, List<string>> subSection in formatLines)
			{
				result += $"\n{divider}";
				string[] headerLines = Text.Wrap(subSection.Key, wrapwidth-13).Split('\n');
				for(int i = 0;i < headerLines.Length;i++)
				{
					string headerLine = headerLines[i];
					int padNeeded = wrapwidth - (StripColours(headerLine).Length+7);
					int padLeft = padNeeded/2;
					int padRight = padNeeded-padLeft;
					result += $"\n {sideBar}{new String(' ', padLeft)}{Colours.Fg(headerLine, Colours.BoldCyan)}{new String(' ', padRight)}{sideBar}";
				}
				result += $"\n{divider}";
				result += $"\n{emptyLine}";

				foreach(string line in subSection.Value)
				{
					string[] subLines = Wrap(line, wrapwidth-13).Split("\n");
					for(int i = 0;i<subLines.Length;i++)
					{
						string subLine = subLines[i];
						result += $"\n {sideBar}   {Colours.Fg(subLine, Colours.BoldWhite)}{new String(' ', wrapwidth - StripColours(subLine).Length - 10)}{sideBar}";
					}
				}
				result += $"\n{emptyLine}";
			}
			result += $"\n{divider}";
			return result;
		}
	}
}