using System;
using System.Diagnostics;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdAddroom(GameObject invoker, CommandData cmd)
		{
			if(invoker.location == null || !invoker.location.HasComponent(Text.CompRoom))
			{
				invoker.WriteLine("This command is only usable within a room.");
			}
			else
			{
				if(cmd.objTarget == null)
				{
					invoker.WriteLine("Which exit do you wish to add a room to?");
				}
				else if(cmd.strArgs.Length < 1)
				{
					invoker.WriteLine("Please specify a valid room ID to link to, or 'new' to use a new room.");
				}
				else
				{
					string exitToAdd = cmd.objTarget;
					if(Text.shortExits.ContainsKey(exitToAdd))
					{
						exitToAdd = Text.shortExits[exitToAdd];
					}
					RoomComponent room = (RoomComponent)invoker.location.GetComponent(Text.CompRoom);
					if(room.exits.ContainsKey(exitToAdd))
					{
						invoker.WriteLine($"There is already an exit to the {exitToAdd} in this room.");
					}
					else
					{
						long roomId = -1;
						Console.WriteLine(cmd.strArgs[1]);
						if(cmd.strArgs[1].ToLower() == "new")
						{
							roomId = Modules.Templates.Instantiate("room").id;
						}
						else
						{
							try
							{
								roomId = Int32.Parse(cmd.strArgs[0].ToLower());
							}
							catch(Exception e)
							{
								Debug.WriteLine($"Room ID exception: {e.ToString()}.");
							}
						}
						if(roomId == -1 || Game.Objects.Get(roomId) == null)
						{
							invoker.WriteLine("Please specify a valid room ID to link to, or 'new' to use a new room.");
						}
						else
						{
							bool saveEditedRoom = true;
							GameObject linkingRoom = (GameObject)Game.Objects.Get(roomId);
							if((cmd.strArgs.Length >= 2 && cmd.strArgs[1].ToLower() == "one-way") || !linkingRoom.HasComponent(Text.CompRoom) || !Text.reversedExits.ContainsKey(exitToAdd))
							{
								room.exits.Add(exitToAdd, roomId);
								saveEditedRoom = true;
								invoker.WriteLine($"You have connected {room.parent.id} to {roomId} via a one-way exit to the {exitToAdd}.");
							}
							else
							{
								string otherExit = Text.reversedExits[exitToAdd];
								RoomComponent otherRoom = (RoomComponent)linkingRoom.GetComponent(Text.CompRoom);
								if(otherRoom.exits.ContainsKey(otherExit))
								{
									room.exits.Add(exitToAdd, roomId);
									saveEditedRoom = true;
									invoker.WriteLine($"Target room already has an exit to the {otherExit}.\nYou have connected {room.parent.id} to {roomId} via a one-way exit to the {exitToAdd}.");
								}
								else
								{
									room.exits.Add(exitToAdd, roomId);
									saveEditedRoom = true;
									otherRoom.exits.Add(otherExit, room.parent.id);
									Game.Objects.QueueForUpdate(otherRoom.parent);
									invoker.WriteLine($"You have connected {room.parent.id} to {roomId} via a bidirectional exit to the {exitToAdd}.");
								}
							}
							if(saveEditedRoom)
							{
								Game.Objects.QueueForUpdate(room.parent);
							}
						}
					}
				}
			}
			invoker.SendPrompt();
		}
	}
}