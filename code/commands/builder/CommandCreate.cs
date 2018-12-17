using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command 
	{
		internal static void CmdCreate(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length > 0)
			{
				string token = tokens[0].ToLower();
				GameObject hat = Templates.Instantiate(token);
				if(hat != null)
				{
					hat.Move(invoker.shell.location);
					invoker.WriteLine($"Created {hat.GetString(Components.Visible, Text.FieldShortDesc)} ({hat.name}#{hat.id}).");
					invoker.SendPrompt();
					return;
				}
			}
			invoker.WriteLine($"You can create the following: {Text.EnglishList(Templates.GetTemplateNames())}");
			invoker.SendPrompt();
		}
	}
}