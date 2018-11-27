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
			invoker.WriteToStream($"You emote: {invoker.clientId} {invocation}");
			string showToOthers = $"{invoker.clientId} {invocation}";
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