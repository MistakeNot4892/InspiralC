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
			AddCommand(new CommandCreate());
		}
		internal override void OnContextSet(GameClient viewer)
		{
			viewer.WriteLine($"Welcome, {viewer.currentGameObject.GetString(Text.FieldShortDesc)}.");
			if(Game.Rooms.spawnRoom == null)
			{
				viewer.WriteLine("Room is null!");
			}
			else
			{
				viewer.currentGameObject.Move(Game.Rooms.spawnRoom);
			}
		}
		internal override string GetPrompt(GameClient viewer) 
		{
			return "\n>";
		}
	}
}