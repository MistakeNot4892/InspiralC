using System;
using System.Collections.Generic;

namespace inspiral
{
	class CommandLook : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "look";
			aliases = new List<string>() {"look", "l", "ql"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0 || tokens[0] == "")
			{
				if(invoker.shell.location != null)
				{
					invoker.shell.location.ExaminedBy(invoker, true);
				}
				else
				{
					invoker.WriteLinePrompted("You cannot see anything here.");
				}
			}
			else
			{
				if(tokens.Length >= 1)
				{
					string lookingAt = tokens[0].ToLower();
					if(lookingAt == "at" && tokens.Length >= 2)
					{
						lookingAt = tokens[1].ToLower();
					}
					if(lookingAt == "me")
					{
						invoker.shell.ExaminedBy(invoker, false);
						return true;
					}
					else if(invoker.shell.location != null && invoker.shell.location.HasComponent(Components.Room))
					{
						RoomComponent room = (RoomComponent)invoker.shell.location.GetComponent(Components.Room);
						if(Text.shortExits.ContainsKey(lookingAt))
						{
							lookingAt = Text.shortExits[lookingAt];
						}
						if(room.exits.ContainsKey(lookingAt))
						{
							GameObject otherRoom = (GameObject)Game.Objects.Get(room.exits[lookingAt]);
							if(otherRoom != null)
							{
								invoker.WriteLine($"You gaze off to the {lookingAt} and see:");
								otherRoom.ExaminedBy(invoker, false);
								return true;
							}
						}
						if(Text.exits.Contains(lookingAt))
						{
							invoker.WriteLinePrompted("You can see nothing in that direction.");
							return true;
						}
					}
					GameObject examining = invoker.shell.location.FindInContents(lookingAt);
					if(examining != null)
					{
						examining.ExaminedBy(invoker, false);
					}
					else
					{
						invoker.WriteLinePrompted("You can see nothing here by that name.");
					}
				}
			}
			return true;
		}
	}
}