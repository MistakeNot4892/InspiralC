using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdRoles(GameObject invoker, CommandData cmd)
		{
			if(cmd.objTarget == null)
			{
				invoker.WriteLine("Who do you wish to view the roles of?", true);
				return;
			}

			PlayerAccount acct = Game.Accounts.FindAccount(cmd.objTarget);
			if(acct == null)
			{
				invoker.WriteLine($"Cannot find account for '{cmd.objTarget}'.", true);
				return;
			}

			string header = $"Roles for {acct.userName}";
			Dictionary<string, List<string>> roleDetails = new Dictionary<string, List<string>>();
			roleDetails.Add(header, new List<string>());

			int wrap = 80;
			if(invoker.HasComponent(Text.CompClient))
			{
				ClientComponent client = (ClientComponent)invoker.GetComponent(Text.CompClient);
				wrap = client.client.config.wrapwidth;
			}

			foreach(GameRole role in acct.roles)
			{
				roleDetails[header].Add(Text.FormatPopup(role.name, role.GetSummary(), wrap + Text.NestedWrapwidthModifier));
			}
			invoker.WriteLine(Text.FormatBlock(roleDetails, wrap), true);
		}
	}
}