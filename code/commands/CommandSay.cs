using System.Collections.Generic;

namespace inspiral
{
	class CommandSay : GameCommand
	{
		internal CommandSay() 
		{
			commandString = "say";
			aliases = new List<string>() {"say", "'"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			if(invocation.Length <= 0)
			{
				invoker.shell.ShowNearby(invoker.shell, $"You open your mouth but say nothing.", $"{invoker.shell.GetString(Components.Visible, Text.FieldShortDesc)} opens their mouth but says nothing.");
			}
			else
			{
				invocation = Text.FormatProse(invocation);
				invoker.shell.ShowNearby(invoker.shell, $"You say, \"{invocation}\"", $"{invoker.shell.GetString(Components.Visible, Text.FieldShortDesc)} says, \"{invocation}\"");
			}
			return true;
		}
	}
}