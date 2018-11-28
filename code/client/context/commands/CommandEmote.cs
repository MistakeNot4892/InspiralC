using System.Collections.Generic;

namespace inspiral
{
	class CommandEmote : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "emote";
			aliases = new List<string>() {"emote", "em", "me"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string emoteText = GameText.FormatProse($"{invoker.currentGameObject.GetString("short_description")} {invocation}");
			invoker.currentGameObject.ShowNearby(invoker.currentGameObject, $"You emote: {emoteText}", emoteText);
			return true;
		}
	}
}