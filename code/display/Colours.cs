using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static class Colours
	{
		internal const string Black =       "0;30m";
		internal const string Red =         "0;31m";
		internal const string Green =       "0;32m";
		internal const string Yellow =      "0;33m";
		internal const string Blue =        "0;34m";
		internal const string Magenta =     "0;35m";
		internal const string Cyan =        "0;36m";
		internal const string White =       "0;37m";
		internal const string BoldBlack =   "1;30m";
		internal const string BoldRed =     "1;31m";
		internal const string BoldGreen =   "1;32m";
		internal const string BoldYellow =  "1;33m";
		internal const string BoldBlue =    "1;34m";
		internal const string BoldMagenta = "1;35m";
		internal const string BoldCyan =    "1;36m";
		internal const string BoldWhite =   "1;37m";
		internal const string Reset =       "0m";
		internal const string ColourCodeIndicator = "\u001b[";

		internal static string Fg(string message, string code)
		{
			return $"{ColourCodeIndicator}{code}{message}{ColourCodeIndicator}{Reset}";
		}
	}
}