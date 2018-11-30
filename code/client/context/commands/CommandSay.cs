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
			if(invocation.Length <= 0)
			{
				invoker.currentGameObject.ShowNearby(invoker.currentGameObject, $"You open your mouth but say nothing.", $"{invoker.currentGameObject.GetString(Text.FieldShortDesc)} opens their mouth but says nothing.");
			}
			else
			{
				invocation = Text.FormatProse(invocation);
				invoker.currentGameObject.ShowNearby(invoker.currentGameObject, $"You say, \"{invocation}\"", $"{invoker.currentGameObject.GetString(Text.FieldShortDesc)} says, \"{invocation}\"");
			}
			return true;
		}
	}
}