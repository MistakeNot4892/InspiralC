using System.Collections.Generic;

namespace inspiral
{
	class CommandSet : GameCommand
	{
		internal CommandSet() 
		{
			commandString = "set";
			aliases = new List<string>() {"vs"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length < 3)
			{
				invoker.SendLineWithPrompt("Usage: SET <target> <field> <value>");
				return true;
			}
			GameObject editing = invoker.shell.FindGameObjectNearby(tokens[0].ToLower());
			if(editing == null)
			{
				invoker.SendLineWithPrompt("Cannot find object to modify.");
				return true;
			}
			editing.EditValue(invoker, tokens[1].ToLower(), invocation.Substring(tokens[0].Length + tokens[1].Length + 2));
			return true;
		}
	}
}