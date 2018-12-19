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
		internal void CmdTest(GameObject invoker, CommandData cmd) 
		{
			invoker.SendLine(cmd.GetSummary());
		}
	}
}