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

		internal const string DefaultRoomShort =           "an empty room";
		internal const string DefaultRoomLong =            "This is a completely empty room.";
		internal const string DefaultName =                "object";
		internal const string DefaultShortDescription =    "a generic object";
		internal const string DefaultRoomDescription =     "A generic object is here.";
		internal const string DefaultExaminedDescription = "This is a generic object. Fascinating stuff.";
		internal const string DefaultEnterMessage =        "A generic object enters from the $DIR.";
		internal const string DefaultLeaveMessage =        "A generic object leaves to the $DIR";
		internal const string DefaultDeathMessage =        "A generic object lies here, dead.";
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