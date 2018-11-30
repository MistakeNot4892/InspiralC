using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static class Colours
	{
		internal const int Black =       0;
		internal const int Red =         1;
		internal const int Green =       2;
		internal const int Yellow =      3;
		internal const int Blue =        4;
		internal const int Magenta =     5;
		internal const int Cyan =        6;
		internal const int White =       7;
		internal const int BoldBlack =   8;
		internal const int BoldRed =     9;
		internal const int BoldGreen =   10;
		internal const int BoldYellow =  11;
		internal const int BoldBlue =    12;
		internal const int BoldMagenta = 13;
		internal const int BoldCyan =    14;
		internal const int BoldWhite =   15;

		internal static string Fg(string message, int code)
		{
			return message; //$"\u001b[38;5;${code}m{message}\u001b[0m";
		}
		internal static void ShowTo(GameClient invoker)
		{
			string line = "";
			for(int k = 0;k<16;k++)
			{
				line = "";
				for(int j = 0;j<16;j++)
				{
					int code = (k * 16) + j;
					line = $"{line}{new String(' ', 6 - code.ToString().Length)}{Fg(code.ToString(), code)}";
				}
				invoker.WriteLine(line);
			}
			invoker.WriteLinePrompted("");
		}
	}
}