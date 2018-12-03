using System;
using System.Collections.Generic;

namespace inspiral
{
	class CommandLook : GameCommand
	{
		internal CommandLook() 
		{
			commandString = "look";
			aliases = new List<string>() {"look", "l", "ql"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			if(invoker.shell.location == null)
			{
				invoker.SendLineWithPrompt("You cannot see anything here.");
				return true;
			}
			GameObject examining = null;
			string[] tokens = invocation.Split(" ");
			if(tokens.Length >= 2 && tokens[0] == "at")
			{
				examining = invoker.shell.FindGameObjectNearby(tokens[1].ToLower());
			}
			else if(tokens.Length >= 1 && tokens[0] != "")
			{
				examining = invoker.shell.FindGameObjectNearby(tokens[0].ToLower());
			}
			else 
			{
				examining = invoker.shell.FindGameObjectNearby("here");
			}
			if(examining != null)
			{
				examining.ExaminedBy(invoker, false);
			}
			else
			{
				invoker.SendLineWithPrompt("You can see nothing here by that name.");
			}
			return true;
		}
	}
}