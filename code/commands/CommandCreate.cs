using System;
using System.Collections.Generic;

namespace inspiral
{
	class CommandCreate : GameCommand
	{
		internal CommandCreate() 
		{
			commandString = "create";
			aliases = new List<string>() {"create"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0)
			{
				invoker.WriteLinePrompted("What do you want to create (object, room)?");
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
						invoker.WriteLinePrompted($"Created {gameObject.GetString(Components.Visible, Text.FieldShortDesc)} (#{gameObject.id}).");
						break;
					case "room":
						GameObject room = (GameObject)Game.Objects.Get(Game.Objects.CreateNewEmptyRoom());
						invoker.WriteLinePrompted($"Created {room.GetString(Components.Visible, Text.FieldShortDesc)} (#{room.id}).");
						break;
					default:
						invoker.WriteLinePrompted("What do you want to create (object, room)?");
						break;
				}
			}
			return true;
		}
	}
}