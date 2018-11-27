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
			invoker.WriteToStream($"You say, \"{invocation}\"");
			string showToOthers = $"{invoker.clientId} says, \"{invocation}\"";

			foreach(GameClient client in Program.clients)
			{
				if(invoker.clientId != client.clientId)
				{
					client.WriteToStream(showToOthers);
				}
			}
			return true;
		}
	}
}