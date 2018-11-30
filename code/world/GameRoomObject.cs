using System;
using System.Collections.Generic;

namespace inspiral
{

	internal class RoomExit
	{
		string descriptor;
		long connects;
	}

	class GameRoomObject : GameObject
	{
		Dictionary<string, RoomExit> exits;
		internal GameRoomObject()
		{
			SetString(Text.FieldShortDesc, "an empty room");
			SetString(Text.FieldExaminedDesc, "This is a generic empty room. Incredible.");
			exits = new Dictionary<string, RoomExit>();
		}
	}
}
