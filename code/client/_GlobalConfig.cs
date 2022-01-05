namespace inspiral
{
    internal static class GlobalConfig
    {
        internal static string DefaultPlayerGender = Text.GenderInanimate;
        internal static string DefaultShellTemplate = "mob";

        // String replacement tokens
        // TODO maybe use $(FOO$) like Aet and use a regex/switch?
        internal static string StringTokenDir = "$dir$";
        internal static string StringTokenShort = "$short$";
        internal static string StringTokenShortCaps = "$Short$";

        // Default colours
        internal static string DefaultColour =                  Colours.White;
        internal static string DefaultColourSubtle =            Colours.BoldBlack;
        internal static string DefaultColourHighlight =         Colours.BoldWhite;
		internal static string DefaultColourExits =             Colours.BoldCyan;
		internal static string DefaultColourFrameHighlight =    Colours.BoldCyan;
		internal static string DefaultColourFramePrimary =      Colours.Cyan;
		internal static string DefaultColourFrameSecondary =    Colours.Blue;
        internal static string DefaultColourBleeding =          Colours.Red;
        internal static string DefaultColourBleedingHighlight = Colours.BoldRed;
        internal static string DefaultColourPrompt =            Colours.Yellow;
        internal static string DefaultColourPromptHighlight =   Colours.BoldYellow;
        internal static string DefaultColourPain =              Colours.Yellow;
        internal static string DefaultColourPainHighlight =     Colours.BoldYellow;
		internal static string GetColour(string colourType)
		{
			switch(colourType)
			{
				case Text.ColourDefaultSubtle:
					return GlobalConfig.DefaultColourSubtle;
				case Text.ColourDefaultPrompt:
					return GlobalConfig.DefaultColourPrompt;
				case Text.ColourDefaultPromptHighlight:
					return GlobalConfig.DefaultColourPromptHighlight;
				case Text.ColourDefaultBleeding:
					return GlobalConfig.DefaultColourBleeding;
				case Text.ColourDefaultBleedingHighlight:
					return GlobalConfig.DefaultColourBleedingHighlight;
				case Text.ColourDefaultPain:
					return GlobalConfig.DefaultColourPain;
				case Text.ColourDefaultPainHighlight:
					return GlobalConfig.DefaultColourPainHighlight;
				case Text.ColourDefaultExits:
					return GlobalConfig.DefaultColourExits;
				case Text.ColourDefaultFrameHighlight:
					return GlobalConfig.DefaultColourFrameHighlight;
				case Text.ColourDefaultFramePrimary:
					return GlobalConfig.DefaultColourFramePrimary;
				case Text.ColourDefaultFrameSecondary:
					return GlobalConfig.DefaultColourFrameSecondary;
				case Text.ColourDefaultHighlight:
					return GlobalConfig.DefaultColourHighlight;
				default:
					return GlobalConfig.DefaultColour;
			}
        }
    } 

	internal static partial class Text
	{
		internal const string ColourDefaultSubtle =            "subtle";
		internal const string ColourDefaultPrompt =            "prompt";
		internal const string ColourDefaultPromptHighlight =   "prompt highlight";
		internal const string ColourDefaultBleeding =          "blood";
		internal const string ColourDefaultBleedingHighlight = "blood highlight";
		internal const string ColourDefaultPain =              "pain";
		internal const string ColourDefaultPainHighlight =     "pain highlight";
		internal const string ColourDefaultExits =             "exits";
		internal const string ColourDefaultFrameHighlight =    "border highlight";
		internal const string ColourDefaultFramePrimary =      "border primary";
		internal const string ColourDefaultFrameSecondary =    "border secondary";
		internal const string ColourDefaultHighlight =         "default highlight";
	}
}