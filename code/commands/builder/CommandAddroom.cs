namespace inspiral
{
	internal class CommandAddRoom : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("addroom");
			Aliases.Add("connect");
			Description = "Adds a new exit to a room.";
			Usage = "addroom [direction] [room id or 'new'] <one-way>";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			var roomComp = invoker.Location?.GetComponent<RoomComponent>();
			if(roomComp == null)
			{
				invoker.WriteLine("This command is only usable within a room.");
			}
			else
			{
				if(cmd.ObjTarget == null)
				{
					invoker.WriteLine("Which exit do you wish to add a room to?");
				}
				else if(cmd.StrArgs.Length < 1)
				{
					invoker.WriteLine("Please specify a valid room ID to link to, or 'new' to use a new room.");
				}
				else
				{
					string exitToAdd = cmd.ObjTarget;
					if(Text.shortExits.ContainsKey(exitToAdd))
					{
						exitToAdd = Text.shortExits[exitToAdd];
					}
					RoomComponent room = (RoomComponent)roomComp;
					if(room.exits.ContainsKey(exitToAdd))
					{
						invoker.WriteLine($"There is already an exit to the {exitToAdd} in this room.");
					}
					else
					{
						long roomId = -1;
						System.Console.WriteLine(cmd.StrArgs[1]);
						if(cmd.StrArgs[1].ToLower() == "new")
						{
							roomId = Game.Modules.Rooms.CreateEmpty().GetValue<long>(Field.Id);
						}
						else
						{
							try
							{
								roomId = System.Int32.Parse(cmd.StrArgs[0].ToLower());
							}
							catch(System.Exception e)
							{
								Game.LogError($"Room ID exception: {e.ToString()}.");
							}
						}

						var getRoom = Game.Repositories.Objects.GetById(roomId);
						if(roomId == -1 || getRoom == null || room.parent == null)
						{
							invoker.WriteLine("Please specify a valid room ID to link to, or 'new' to use a new room.");
						}
						else
						{
							bool saveEditedRoom = true;
							GameObject linkingRoom = (GameObject)getRoom;
							if((cmd.StrArgs.Length >= 2 && cmd.StrArgs[1].ToLower() == "one-way") || !linkingRoom.HasComponent<RoomComponent>() || !Text.reversedExits.ContainsKey(exitToAdd))
							{
								room.exits.Add(exitToAdd, roomId);
								saveEditedRoom = true;
								invoker.WriteLine($"You have connected {room.parent.GetValue<long>(Field.Id)} to {roomId} via a one-way exit to the {exitToAdd}.");
							}
							else
							{
								string otherExit = Text.reversedExits[exitToAdd];
								var otherRoomComp = linkingRoom.GetComponent<RoomComponent>();
								if(otherRoomComp != null)
								{
									RoomComponent otherRoom = (RoomComponent)otherRoomComp;
									if(otherRoom.exits.ContainsKey(otherExit))
									{
										room.exits.Add(exitToAdd, roomId);
										saveEditedRoom = true;
										invoker.WriteLine($"Target room already has an exit to the {otherExit}.\nYou have connected {room.parent.GetValue<long>(Field.Id)} to {roomId} via a one-way exit to the {exitToAdd}.");
									}
									else
									{
										room.exits.Add(exitToAdd, roomId);
										saveEditedRoom = true;
										otherRoom.exits.Add(otherExit, room.parent.GetValue<long>(Field.Id));
										if(otherRoom.parent != null)
										{
											Game.Repositories.Objects.QueueForUpdate(otherRoom.parent);
										}
										invoker.WriteLine($"You have connected {room.parent.GetValue<long>(Field.Id)} to {roomId} via a bidirectional exit to the {exitToAdd}.");
									}
								}
							}
							if(saveEditedRoom)
							{
								Game.Repositories.Objects.QueueForUpdate(room.parent);
							}
						}
					}
				}
			}
		}
	}
}