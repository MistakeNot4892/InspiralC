using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inspiral
{
	class Program
	{
		internal static List<GameClient> clients;
		static void Main(string[] args)
		{
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
