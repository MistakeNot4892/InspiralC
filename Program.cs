using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inspiral
{
	class Program
	{
		internal static GameObject spawnRoom;
		internal static List<GameClient> clients;
		static void Main(string[] args)
		{

			// Create plateholder world.
			spawnRoom = new GameObject();
			spawnRoom.SetString("short_description",    "An empty room.");
			spawnRoom.SetString("room_description",     "There is a perfectly generic white building here.");
			spawnRoom.SetString("examined_description", "This is a large, spacious, empty room with white floors and walls. There is no ceiling. Do not think about the ceiling.");

			GameObject propOne = new GameObject();
			propOne.SetString("short_description",    "a #1 prop");
			propOne.SetString("room_description",     "There is a generic object here.");
			propOne.SetString("examined_description", "This is a weird little entity with a #1 stamped on it.");

			GameObject propTwo = new GameObject();
			propTwo.SetString("short_description",    "a #2 prop");
			propTwo.SetString("room_description",     "There is a very generic object here.");
			propTwo.SetString("examined_description", "This is a very weird little entity with a #2 stamped on it.");

			GameObject propThree = new GameObject();
			propThree.SetString("short_description",    "a #3 prop");
			propThree.SetString("room_description",     "There is a very, VERY generic object here.");
			propThree.SetString("examined_description", "This is a very, VERY weird little entity with a #3 stamped on it.");

			propOne.Move(spawnRoom);
			propTwo.Move(spawnRoom);
			propThree.Move(spawnRoom);

			// Create clients and port listeners.
			clients = new List<GameClient>();
			List<int> ports = new List<int>() {9090, 2323};
			foreach(int port in ports)
			{
				PortListener portListener = new PortListener(port);
				Task.Run(() => portListener.Begin());
			}
			Console.WriteLine("\nHit enter to end run.");
			Console.Read();
		}
	}
}
