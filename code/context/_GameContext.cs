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
		internal virtual bool InvokeCommand(GameClient invoker, string command, string arguments) 
		{
			foreach(GameRole role in invoker.account.roles)
			{
				if(role.AllCommands.ContainsKey(command))
				{
					CommandData cmd = new CommandData(command, arguments);
					role.AllCommands[command].InvokeCommand(invoker.shell, cmd);
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