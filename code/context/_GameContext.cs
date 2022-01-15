namespace inspiral
{
	class GameContext
	{
		internal virtual bool TakeInput(GameClient invoker, string command, string rawCommand, string arguments) { 
			invoker.SendPrompt(); 
			return false; 
		}
		internal virtual void OnContextSet(GameClient viewer) {}
		internal virtual void OnContextUnset(GameClient viewer) {}
		internal virtual bool InvokeCommand(GameClient invoker, string cmdStr, string arguments) 
		{
			foreach(GameRole role in invoker.account.roles)
			{
				if(role.AllCommands.ContainsKey(cmdStr))
				{
					GameCommand cmd = role.AllCommands[cmdStr];
					CommandData cmdData = new CommandData(cmd, cmdStr, arguments);
					cmd.InvokeCommand(invoker.shell, cmdData);
					invoker.SendPrompt();
					return true;
				}
			}
			return false;
		}
		internal virtual string GetPrompt(GameClient viewer) 
		{
			return null;
		}
	}
}