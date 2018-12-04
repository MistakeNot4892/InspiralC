using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static class Commands
	{
		internal static CommandAddroom Addroom = new CommandAddroom();
		internal static CommandClient  Client =  new CommandClient();
		internal static CommandCreate  Create =  new CommandCreate();
		internal static CommandSet     Set =     new CommandSet();
		internal static CommandView    View =    new CommandView();
		internal static CommandEmote   Emote =   new CommandEmote();
		internal static CommandLook    Look =    new CommandLook();
		internal static CommandQuit    Quit =    new CommandQuit();
		internal static CommandSay     Say =     new CommandSay();
		internal static CommandTest    Test =    new CommandTest();
	}
}