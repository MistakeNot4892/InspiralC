using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static bool CmdTakerole(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.ToLower().Split(" ");
			if(tokens.Length < 1)
			{
				invoker.SendLineWithPrompt("Who do you wish to view the roles of?");
				return true;
			}
			else if(tokens.Length < 2)
			{
				invoker.SendLineWithPrompt("Which role do you wish to add?");
				return true;
			}

			PlayerAccount acct = Game.Accounts.FindAccount(tokens[0].ToLower());
			if(acct == null)
			{
				invoker.SendLineWithPrompt($"Cannot find account for '{tokens[0]}'.");
				return true;
			}

			GameRole role = Roles.GetRole(tokens[1].ToLower());
			if(role == null)
			{
				invoker.SendLineWithPrompt($"Cannot find role for '{tokens[1]}'.");
			}
			else if(!acct.roles.Contains(role))
			{
				invoker.SendLineWithPrompt($"They do not have that role.");
			}
			else
			{
				acct.roles.Remove(role);
				Game.Accounts.QueueForUpdate(acct);
				invoker.SendLineWithPrompt($"Removed role '{role.name}' from '{acct.userName}'.");
			}
			return true;
		}
	}
}