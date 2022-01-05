namespace inspiral
{
    internal static class GlobalConfig
    {
        internal static string DefaultPlayerGender = Text.GenderInanimate;
        internal static string DefaultShellTemplate = "mob";

        // TODO maybe use $(FOO$) like Aet and use a regex/switch?
        internal static string StringTokenDir = "$dir$";
        internal static string StringTokenShort = "$short$";
        internal static string StringTokenShortCaps = "$Short$";
    } 
}