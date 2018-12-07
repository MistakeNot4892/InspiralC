using System;
using System.Collections.Generic;

namespace inspiral
{
	class GameContext
	{
		internal virtual bool TakeInput(GameClient invoker, string command, string rawCommand, string arguments) { return false; }
		internal virtual void OnContextSet(GameClient viewer) {}
		internal virtual void OnContextUnset(GameClient viewer) {}
		internal virtual bool InvokeCommand(GameClient invoker, string command, string arguments) 
		{
			foreach(GameRole role in invoker.account.roles)
			{
				if(role.AllCommands.ContainsKey(command))
				{
					GameCommand cmd = role.AllCommands[command];
					if(cmd.invokedMethod != null)
					{
						return (bool)cmd.invokedMethod.Invoke(null, new object[] { invoker, arguments });
					}
				}
			}
			return false;
		}
		internal virtual string GetPrompt(GameClient viewer) 
		{
			return "";
		}
	}
}