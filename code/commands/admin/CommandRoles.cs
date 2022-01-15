using System.Collections.Generic;

namespace inspiral
{
	internal class CommandViewRoles : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new List<string>() { "roles" };
			description = "Shows the details of roles attached to an account.";
			usage = "roles [account name or id]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.ObjTarget == null)
			{
				invoker.WriteLine("Who do you wish to view the roles of?", true);
				return;
			}

			PlayerAccount acct = Game.Accounts.FindAccount(cmd.ObjTarget);
			if(acct == null)
			{
				invoker.WriteLine($"Cannot find account for '{cmd.ObjTarget}'.", true);
				return;
			}

			string header = $"Roles for {acct.userName}";
			Dictionary<string, List<string>> roleDetails = new Dictionary<string, List<string>>();
			roleDetails.Add(header, new List<string>());

			int wrap = 80;
			if(invoker.HasComponent<ClientComponent>())
			{
				ClientComponent client = (ClientComponent)invoker.GetComponent<ClientComponent>();
				wrap = client.client.config.wrapwidth;
			}

			foreach(GameRole role in acct.roles)
			{
				roleDetails[header].Add(Text.FormatPopup(invoker, role.name, role.GetSummary(), wrap + Text.NestedWrapwidthModifier));
			}
			invoker.WriteLine(Text.FormatBlock(invoker, roleDetails, wrap), true);
		}
	}
}