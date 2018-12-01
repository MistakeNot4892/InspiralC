using System;
using System.Collections.Generic;

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

		internal static string FormatPopup(string header, List<string> boxContents)
		{
			int maxLine = 70;
			int totalLine = 80;
			if(header.Length > maxLine)
			{
				header = $"{header.Substring(0, maxLine-3)}...";
			}
			List<string> splitContents = new List<string>();
			foreach(string splitLine in boxContents)
			{
				string pruneLine = splitLine;
				while(pruneLine.Length > maxLine)
				{
					splitContents.Add(pruneLine.Substring(0, maxLine));
					pruneLine = pruneLine.Substring(maxLine);
				}
				splitContents.Add(pruneLine);
			}

			List<string> result = new List<string>();

			int padNeeded = totalLine - (header.Length + 6);
			int padLeft = padNeeded/2;
			int padRight = padLeft;
			while(padLeft + padRight != padNeeded)
			{
				padRight++;
			}
			result.Add($"{Colours.Fg($"[{new String('=', padLeft)}\\", Colours.Cyan)} {Colours.Fg(header, Colours.BoldCyan)} {Colours.Fg($"/{new String('=', padRight)}]",Colours.Cyan)}");
			string emptyLine = $" {sideBar}{new String(' ', totalLine-6)}{sideBar}";
			result.Add(emptyLine);
			foreach(string splitLine in splitContents)
			{
				result.Add($" {sideBar}     {Colours.Fg(splitLine, Colours.BoldWhite)}{new String(' ', totalLine - splitLine.Length - 11)}{sideBar}");
			}
			result.Add(emptyLine);
			result.Add(Colours.Fg($"[{new String('=', totalLine-2)}]", Colours.Cyan));
			return string.Join("\n", result.ToArray());
		}


		internal static string FormatBlock(Dictionary<string, List<string>> formatLines)
		{

			int longestLine = 30;
			foreach(KeyValuePair<string, List<string>> subSection in formatLines)
			{
				if(subSection.Key.Length > longestLine)
				{
					longestLine = subSection.Key.Length;
				}
				foreach(string line in subSection.Value)
				{
					if(line.Length > longestLine)
					{
						longestLine = line.Length;
					}
				}
			}
			longestLine += 5;

			string divider = Colours.Fg($"[{new String('=', longestLine-2)}]", Colours.Cyan);
			string emptyLine = $" {sideBar}{new String(' ', longestLine-6)}{sideBar}";
	
			List<string> result = new List<string>();

			foreach(KeyValuePair<string, List<string>> subSection in formatLines)
			{
				int padNeeded = longestLine - (subSection.Key.Length + 6);
				int padLeft = padNeeded/2;
				int padRight = padLeft;
				while(padLeft + padRight != padNeeded)
				{
					padRight++;
				}

				result.Add(divider);
				result.Add($" {sideBar}{new String(' ', padLeft)}{Colours.Fg(subSection.Key, Colours.BoldCyan)}{new String(' ', padRight)}{sideBar}");
				result.Add(divider);
				result.Add(emptyLine);
				foreach(string line in subSection.Value)
				{
					result.Add($" {sideBar}     {Colours.Fg(line, Colours.BoldWhite)}{new String(' ', longestLine - line.Length - 11)}{sideBar}");
				}
				result.Add(emptyLine);
			}
			result.Add(divider);
			return string.Join("\n", result.ToArray());
		}
	}
}