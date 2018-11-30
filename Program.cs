using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inspiral
{

	class Program
	{
		static void Main(string[] args)
		{
			Game.Load();
			Game.Initialize();
			// Create port listeners.
			List<int> ports = new List<int>() {9090, 2323};
			foreach(int port in ports)
			{
				PortListener portListener = new PortListener(port);
				Task.Run(() => portListener.Begin());
			}
			Console.WriteLine("Hit enter to end run.");
			Console.Read();
		}
	}
}
