using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace inspiral
{
	class CommandAddroom : GameCommand
	{
		internal CommandAddroom() 
		{
			commandString = "addroom";
			aliases = new List<string>() {"addroom"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			if(invoker.shell == null || invoker.shell.location == null || !invoker.shell.location.HasComponent(Components.Room))
			{
				invoker.WriteLinePrompted("This command is only usable within a room.");
			}
			else
			{
				string[] tokens = invocation.Split(" ");
				if(tokens.Length <= 0)
				{
					invoker.WriteLinePrompted("Which exit do you wish to add a room to?");
				}
				else if(tokens.Length <= 1)
				{
					invoker.WriteLinePrompted("Please specify a valid room ID to link to, or 'new' to use a new room.");
				}
				else
				{
					string exitToAdd = tokens[0].ToLower();
					if(Text.shortExits.ContainsKey(exitToAdd))
					{
						exitToAdd = Text.shortExits[exitToAdd];
					}
					RoomComponent room = (RoomComponent)invoker.shell.location.GetComponent(Components.Room);
					if(room.exits.ContainsKey(exitToAdd))
					{
						invoker.WriteLinePrompted($"There is already an exit to the {exitToAdd} in this room.");
					}
					else
					{
						long roomId = -1;
						if(tokens[1].ToLower() == "new")
						{
							roomId = Game.Objects.CreateNewEmptyRoom();
						}
						else
						{
							try
							{
								roomId = Int32.Parse(tokens[1].ToLower());
							}
							catch(Exception e)
							{
								Debug.WriteLine($"Room ID exception: {e.ToString()}.");
							}
						}
						if(roomId == -1 || Game.Objects.Get(roomId) == null)
						{
							invoker.WriteLinePrompted("Please specify a valid room ID to link to, or 'new' to use a new room.");
						}
						else
						{
							GameObject linkingRoom = (GameObject)Game.Objects.Get(roomId);
							if((tokens.Length >= 3 && tokens[2].ToLower() == "one-way") || !linkingRoom.HasComponent(Components.Room) || !Text.reversedExits.ContainsKey(exitToAdd))
							{
								room.exits.Add(exitToAdd, roomId);
								Game.Objects.SaveObject(room.parent);
								invoker.WriteLinePrompted($"You have connected {room.parent.id} to {roomId} via a one-way exit to the {exitToAdd}.");
							}
							else
							{
								string otherExit = Text.reversedExits[exitToAdd];
								RoomComponent otherRoom = (RoomComponent)linkingRoom.GetComponent(Components.Room);
								if(otherRoom.exits.ContainsKey(otherExit))
								{
									room.exits.Add(exitToAdd, roomId);
									Game.Objects.SaveObject(room.parent);
									invoker.WriteLinePrompted($"Target room already has an exit to the {otherExit}.\nYou have connected {room.parent.id} to {roomId} via a one-way exit to the {exitToAdd}.");
								}
								else
								{
									room.exits.Add(exitToAdd, roomId);
									Game.Objects.SaveObject(room.parent);
									otherRoom.exits.Add(otherExit, room.parent.id);
									Game.Objects.SaveObject(otherRoom.parent);
									invoker.WriteLinePrompted($"You have connected {room.parent.id} to {roomId} via a bidirectional exit to the {exitToAdd}.");
								}
							}
						}
					}
				}
			}
			return true;
		}
	}
}