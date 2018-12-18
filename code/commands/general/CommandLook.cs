using System;
using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdLook(GameClient invoker, string invocation)
		{
			if(invoker.shell.location == null)
			{
				invoker.SendLine("You cannot see anything here.");
				return;
			}
			string examineKey = null;
			GameObject examining = null;
			string[] tokens = invocation.Split(" ");
			if(tokens.Length >= 2 && tokens[0] == "at")
			{
				examineKey = tokens[1].ToLower();
			}
			else if(tokens.Length >= 1 && tokens[0] != "")
			{
				examineKey = tokens[0].ToLower();
			}
			else 
			{
				examining = invoker.shell.FindGameObjectNearby("here");
			}
			if(examineKey != null)
			{
				examining = invoker.shell.FindGameObjectNearby(examineKey);
				if(examining == null)
				{
					examining = invoker.shell.FindGameObjectInContents(examineKey);
				}
			}
			if(examining != null)
			{
				examining.ExaminedBy(invoker, false);
			}
			else
			{
				invoker.SendLine("You can see nothing here by that name.");
			}
		}
	}
}