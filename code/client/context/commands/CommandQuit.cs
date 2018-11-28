using System.Collections.Generic;

namespace inspiral
{
	class CommandQuit : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "quit";
			aliases = new List<string>() {"quit", "qq"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			foreach(GameClient client in Program.clients)
			{
				if(invoker.clientId != client.clientId)
				{
					client.WriteLinePrompted($"{invoker.clientId} has disconnected.");
				}
			}
			invoker.Quit();
			return true;
		}
	}
}