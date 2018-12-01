using System.Collections.Generic;

namespace inspiral
{
	class CommandTest : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "test";
			aliases = new List<string>() {"test"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			invoker.WriteLine("Sending test packet.");
			Telnet.SendGMCPPacket(invoker, "Char.Foo.Bar");
			invoker.WriteLine("Done.");
			return true;
		}
	}
}