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
			if(invocation == "")
			{
				invoker.currentGameObject.ShowNearby(invoker.currentGameObject, $"You open your mouth but say nothing.", $"{invoker.currentGameObject.GetString("short_description")} opens their mouth but says nothing.");
			}
			else
			{
				invoker.currentGameObject.ShowNearby(invoker.currentGameObject, $"You say, \"{invocation}\"", $"{invoker.currentGameObject.GetString("short_description")} says, \"{invocation}\"");
			}
			return true;
		}
	}
}