using System.Collections.Generic;

namespace inspiral
{
	class CommandEdit : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "edit";
			aliases = new List<string>() {"edit"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{

			string[] tokens = invocation.Split(" ");
			if(tokens.Length < 3)
			{
				invoker.WriteLinePrompted("Usage: EDIT <target> <field> <value>");
				return true;
			}

			GameObject editing = invoker.shell.FindGameObjectNearby(tokens[0].ToLower());
			if(editing == null)
			{
				invoker.WriteLinePrompted("Cannot find object to edit.");
				return true;
			}
			editing.EditValue(invoker, tokens[1].ToLower(), invocation.Substring(tokens[0].Length + tokens[1].Length + 2));
			return true;
		}
	}
}