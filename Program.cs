namespace inspiral
{

	class Program
	{
		private const string VERSION = "0.0.1b";
		static void Main(string[] args)
		{
			System.Console.WriteLine($"=== Inspiral, Coalescence, Ringdown engine v{VERSION} ===");
			System.Console.WriteLine("Initializing server.");
			Game.Initialize();
			System.Console.WriteLine("Initialization complete.\nServer ready.");
			System.Console.Read();
		}

		static void OnProcessExit(object sender, System.EventArgs e)
		{
			System.Console.WriteLine ("Terminating.");
			Game.Exit();
		}
	}
}
