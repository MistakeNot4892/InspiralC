using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdRoles(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.ToLower().Split(" ");
			if(tokens.Length < 1)
			{
				invoker.SendLineWithPrompt("Who do you wish to view the roles of?");
				return;
			}

			PlayerAccount acct = Game.Accounts.FindAccount(tokens[0]);
			if(acct == null)
			{
				invoker.SendLineWithPrompt($"Cannot find account for '{tokens[0]}'.");
				return;
			}

			string header = $"Roles for {acct.userName}";
			Dictionary<string, List<string>> roleDetails = new Dictionary<string, List<string>>();
			roleDetails.Add(header, new List<string>());
			foreach(GameRole role in acct.roles)
			{
				roleDetails[header].Add(Text.FormatPopup(role.name, role.GetSummary(), invoker.config.wrapwidth + Text.NestedWrapwidthModifier));
			}
			invoker.SendLineWithPrompt(Text.FormatBlock(roleDetails, invoker.config.wrapwidth));
			return;
		}
	}
}