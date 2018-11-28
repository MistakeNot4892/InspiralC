using System;
using System.Collections.Generic;

namespace inspiral
{
	public static class GameColours
	{
		private static Dictionary<string, string> codes;
		static GameColours()
		{
			codes = new Dictionary<string, string>();
			codes.Add("black",        "\u001b[38;5;0m");
			codes.Add("red",          "\u001b[38;5;1m");
			codes.Add("green",        "\u001b[38;5;2m");
			codes.Add("yellow",       "\u001b[38;5;3m");
			codes.Add("blue",         "\u001b[38;5;4m");
			codes.Add("magenta",      "\u001b[38;5;5m");
			codes.Add("cyan",         "\u001b[38;5;6m");
			codes.Add("white",        "\u001b[38;5;7m");
			codes.Add("boldblack",  "\u001b[38;5;8m");
			codes.Add("boldred",    "\u001b[38;5;9m");
			codes.Add("boldgreen",  "\u001b[38;5;10m");
			codes.Add("boldyellow", "\u001b[38;5;11m");
			codes.Add("boldblue",   "\u001b[38;5;12m");
			codes.Add("boldmagenta","\u001b[38;5;13m");
			codes.Add("boldcyan",   "\u001b[38;5;14m");
			codes.Add("boldwhite",  "\u001b[38;5;15m");
		}

		internal static string Fg256(string message, int code)
		{
			if(code > 255)
			{
				code = 255;
			}
			else if(code < 0)
			{
				code = 0;
			}
			return $"\u001b[38;5;${code}m{message}\u001b[0m";
		}
		internal static string Fg(string message, string colour)
		{
			if(codes.ContainsKey(colour.ToLower()))
			{
				return $"{codes[colour]}{message}\u001b[0m";
			}
			return message;
		}
		internal static void ShowTo(GameClient invoker)
		{
			invoker.WriteLine("16 colours:");
			int i = 0;
			string line = "";
			foreach(KeyValuePair<string, string> entry in codes)
			{
				line = $"{line}{new String(' ', 16 - entry.Key.Length)}{entry.Value}{entry.Key}\u001b[0m";
				i++;
				if(i >= 4)
				{
					invoker.WriteLine(line);
					i = 0;
					line = "";
				}
			}
			if(line != "")
			{
				invoker.WriteLine(line);
			}
			invoker.WriteLine("");
			invoker.WriteLine("256 colours:");
			for(int k = 0;k<16;k++)
			{
				line = "";
				for(int j = 0;j<16;j++)
				{
					string code = $"{(k * 16) + j}";
					line = $"{line}{new String(' ', 6 - code.Length)}\u001b[38;5;{code}m{code}\u001b[0m";
				}
				invoker.WriteLine(line);
			}
			invoker.WriteLinePrompted("");
		}
	}
}