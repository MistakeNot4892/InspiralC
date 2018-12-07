using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdTest(GameClient invoker, string invocation)
		{
			MobileComponent mob = (MobileComponent)invoker.shell.GetComponent(Components.Mobile);
			invoker.SendLineWithPrompt(mob.bodyplan.GetSummary());
		}
	}
}