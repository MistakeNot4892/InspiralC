using System.Collections.Generic;

namespace inspiral
{
	class CommandEmote : GameCommand
	{
		internal CommandEmote() 
		{
			commandString = "emote";
			aliases = new List<string>() {"emote", "em", "me"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string emoteText = Text.FormatProse($"{invoker.shell.GetString(Components.Visible, Text.FieldShortDesc)} {invocation}");
			invoker.shell.ShowNearby(invoker.shell, $"You emote: {emoteText}", emoteText);
			return true;
		}
	}
}