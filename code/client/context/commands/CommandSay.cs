using System.Collections.Generic;

namespace inspiral
{
	class CommandSay : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "say";
			aliases = new List<string>() {"say", "s"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invocation = GameText.FormatProse(invocation);
			invoker.currentGameObject.ShowNearby(invoker.currentGameObject, $"You say, \"{invocation}\"", $"{invoker.clientId} says, \"{invocation}\"");
			return true;
		}
	}
}