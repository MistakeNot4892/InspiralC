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
			Console.WriteLine("Server launched. Hit enter to exit.");
			Console.Read();
		}
	}
}
