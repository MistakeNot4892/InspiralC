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
			Console.WriteLine("Inspiral server launched. Hit enter at any time to terminate.");
			Console.Read();
		}

		static void OnProcessExit(object sender, EventArgs e)
		{
			Game.Exit();
			Console.WriteLine ("Terminating.");
		}
	}
}
