using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static class Colours
	{
		internal const string Black =       "0;30";
		internal const string Red =         "0;31";
		internal const string Green =       "0;32";
		internal const string Yellow =      "0;33";
		internal const string Blue =        "0;34";
		internal const string Magenta =     "0;35";
		internal const string Cyan =        "0;36";
		internal const string White =       "0;37";
		internal const string BoldBlack =   "1;30";
		internal const string BoldRed =     "1;31";
		internal const string BoldGreen =   "1;32";
		internal const string BoldYellow =  "1;33";
		internal const string BoldBlue =    "1;34";
		internal const string BoldMagenta = "1;35";
		internal const string BoldCyan =    "1;36";
		internal const string BoldWhite =   "1;37";

		internal static string Fg(string message, string code)
		{
			return $"\u001b[{code}m{message}\u001b[0m";
		}
	}
}