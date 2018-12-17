using System;
using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule 
	{
		internal void CmdCreate(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length > 0)
			{
				string token = tokens[0].ToLower();
				GameObject hat = Modules.Templates.Instantiate(token);
				if(hat != null)
				{
					hat.Move(invoker.shell.location);
					invoker.WriteLine($"Created {hat.GetString(Text.CompVisible, Text.FieldShortDesc)} ({hat.name}#{hat.id}).");
					invoker.SendPrompt();
					return;
				}
			}
			invoker.WriteLine($"You can create the following: {Text.EnglishList(Modules.Templates.GetTemplateNames())}");
			invoker.SendPrompt();
		}
	}
}