using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static class Text
	{
		internal const string FieldShortDesc =    "short_description";
		internal const string FieldRoomDesc =     "room_description";
		internal const string FieldExaminedDesc = "examined_description";
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
	}
}