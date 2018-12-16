using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command 
	{
		internal static void CmdCreate(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0)
			{
				invoker.WriteLine("What do you want to create (object, room)?");
			}
			else
			{
				switch(tokens[0].ToLower())
				{
					case "object":
						GameObject gameObject = (GameObject)Game.Objects.CreateNewInstance(false);
						gameObject.AddComponent(Components.Visible);
						gameObject.Move(invoker.shell.location);
						Game.Objects.AddDatabaseEntry(gameObject);
						invoker.WriteLine($"Created {gameObject.GetString(Components.Visible, Text.FieldShortDesc)} (#{gameObject.id}).");
						break;
					case "room":
						GameObject room = (GameObject)Game.Objects.Get(Game.Objects.CreateNewEmptyRoom());
						invoker.WriteLine($"Created {room.GetString(Components.Visible, Text.FieldShortDesc)} (#{room.id}).");
						break;
					default:
						invoker.WriteLine("What do you want to create (object, room)?");
						break;
				}
			}
			invoker.SendPrompt();
		}
	}
}