using System;
using System.Collections.Generic;

namespace inspiral
{
	class CommandCreate : GameCommand
	{
		internal override string Description { get; set; } = "Creates a new database entry for an object or room.";
		internal override string Command { get; set; } = "create";
		internal override List<string> Aliases { get; set; } = new List<string>() { "create" };
		internal override string Usage { get; set; } = "create [object or room]";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0)
			{
				invoker.SendLineWithPrompt("What do you want to create (object, room)?");
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
						invoker.SendLineWithPrompt($"Created {gameObject.GetString(Components.Visible, Text.FieldShortDesc)} (#{gameObject.id}).");
						break;
					case "room":
						GameObject room = (GameObject)Game.Objects.Get(Game.Objects.CreateNewEmptyRoom());
						invoker.SendLineWithPrompt($"Created {room.GetString(Components.Visible, Text.FieldShortDesc)} (#{room.id}).");
						break;
					default:
						invoker.SendLineWithPrompt("What do you want to create (object, room)?");
						break;
				}
			}
			return true;
		}
	}
}