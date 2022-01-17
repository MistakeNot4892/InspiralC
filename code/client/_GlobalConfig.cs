namespace inspiral
{
    internal static class GlobalConfig
    {
        internal const string DefaultPlayerGender = Text.GenderInanimate;
        internal const string DefaultShellTemplate = "mob";

        // String replacement tokens
        // TODO maybe use $(FOO$) like Aet and use a regex/switch?
        internal const string StringTokenDir = "$dir$";
        internal const string StringTokenShort = "$short$";
        internal const string StringTokenShortCaps = "$Short$";

        // Default colours
        internal const string DefaultColour =                  Colours.White;
        internal const string DefaultColourSubtle =            Colours.BoldBlack;
        internal const string DefaultColourHighlight =         Colours.BoldWhite;
		internal const string DefaultColourExits =             Colours.BoldCyan;
		internal const string DefaultColourFrameHighlight =    Colours.BoldCyan;
		internal const string DefaultColourFramePrimary =      Colours.Cyan;
		internal const string DefaultColourFrameSecondary =    Colours.Blue;
        internal const string DefaultColourBleeding =          Colours.Red;
        internal const string DefaultColourBleedingHighlight = Colours.BoldRed;
        internal const string DefaultColourPrompt =            Colours.Yellow;
        internal const string DefaultColourPromptHighlight =   Colours.BoldYellow;
        internal const string DefaultColourPain =              Colours.Yellow;
        internal const string DefaultColourPainHighlight =     Colours.BoldYellow;
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