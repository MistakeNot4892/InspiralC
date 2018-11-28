using System.Collections.Generic;

namespace inspiral
{
	class ContextGeneral : GameContext
	{
		internal override void Initialize() 
		{
			AddCommand(new CommandSay());
			AddCommand(new CommandEmote());
			AddCommand(new CommandQuit());
			AddCommand(new CommandLook());
			AddCommand(new CommandColours());
		}
		internal override void OnContextSet(GameClient viewer)
		{
			viewer.WriteLinePrompted($"Welcome, {viewer.clientId}.");
			viewer.currentGameObject.Move(Program.spawnRoom);
		}
		internal override string GetPrompt(GameClient viewer) 
		{
			return "\n>";
		}

	}
}