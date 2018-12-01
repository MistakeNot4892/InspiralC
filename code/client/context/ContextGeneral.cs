using System;
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
			viewer.WriteLine($"Welcome, {viewer.shell.name}.");
			if(viewer.shell.location == null)
			{
				Console.WriteLine($"Loc for {viewer.shell.name} was null, moving them to a room.");
				if(Components.Rooms.Count <= 0)
				{
					Console.WriteLine("Cannot find a room, creating a new one.");
					GameObject tmpRoom = (GameObject)Game.Objects.CreateNewInstance(false);
					tmpRoom.AddComponent(Components.Room);
					tmpRoom.AddComponent(Components.Visible);
					tmpRoom.SetString(Components.Visible, Text.FieldShortDesc, Text.DefaultRoomShort);
					tmpRoom.SetString(Components.Visible, Text.FieldExaminedDesc, Text.DefaultRoomLong);
					Game.Objects.AddDatabaseEntry(tmpRoom);
				}

				foreach(GameComponent comp in Components.Rooms)
				{
					Console.WriteLine($"Candidate room #{comp.key} - {comp.parent?.name ?? "null parent"}.");
				}
				GameObject room = (GameObject)Components.Rooms[0].parent;
				Console.WriteLine($"Room id is {room.id}.");
				viewer.shell.Move(room);
			}
		}
		internal override string GetPrompt(GameClient viewer) 
		{
			return "\n>";
		}
	}
}