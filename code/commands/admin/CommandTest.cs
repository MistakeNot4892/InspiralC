using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdTest(GameClient invoker, string invocation) 
		{
			if(invoker.shell.HasComponent(Text.CompMobile))
			{
				MobileComponent mob = (MobileComponent)invoker.shell.GetComponent(Text.CompMobile);
				invoker.shell.WriteLine(mob.bodyplan.GetSummary());
				invoker.SendPrompt();
			}
		}

	}
}