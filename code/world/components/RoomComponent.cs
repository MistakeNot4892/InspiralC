using System;
using System.Collections.Generic;
namespace inspiral
{
	internal class RoomComponent : GameComponent
	{
		internal Dictionary<string, long> exits;

		internal RoomComponent()
		{
			key = Components.Room;
			exits = new Dictionary<string, long>();
		}
	}
}
