using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandRoles Roles = new CommandRoles();
	}

	class CommandRoles : GameCommand
	{
		internal override string Command { get; set; } = "roles";
		internal override List<string> Aliases { get; set; } = new List<string>() { "roles" };
		internal override string Usage { get; set; } = "roles [account name or id]";
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.ToLower().Split(" ");
			if(tokens.Length < 1)
			{
				invoker.SendLineWithPrompt("Who do you wish to view the roles of?");
				return true;
			}

			PlayerAccount acct = Game.Accounts.FindAccount(tokens[0]);
			if(acct == null)
			{
				invoker.SendLineWithPrompt($"Cannot find account for '{tokens[0]}'.");
				return true;
			}

			string header = $"Roles for {acct.userName}";
			Dictionary<string, List<string>> roleDetails = new Dictionary<string, List<string>>();
			roleDetails.Add(header, new List<string>());
			foreach(GameRole role in acct.roles)
			{
				roleDetails[header].Add(Text.FormatPopup(role.name, role.GetSummary(), invoker.config.wrapwidth + Text.NestedWrapwidthModifier));
			}
			invoker.SendLineWithPrompt(Text.FormatBlock(roleDetails, invoker.config.wrapwidth));
			return true;
		}
	}
}