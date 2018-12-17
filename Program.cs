using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inspiral
{

	class Program
	{
		private const string VERSION = "0.0.1b";
		static void Main(string[] args)
		{
			Console.WriteLine($"=== Inspiral, Coalescence, Ringdown engine v{VERSION} ===");
			Console.WriteLine("Initializing server.");
			Game.Initialize();
			Console.WriteLine("Initialization complete.\nServer ready.");
			Console.Read();
		}

		static void OnProcessExit(object sender, EventArgs e)
		{
			Game.Exit();
			Console.WriteLine ("Terminating.");
		}
	}
}
