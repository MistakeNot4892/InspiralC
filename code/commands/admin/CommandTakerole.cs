using System.Collections.Generic;

namespace inspiral
{
	internal class CommandTakeRole : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("takerole");
			Description = "Removes a role from an account.";
			Usage = "takerole [account name or id] [role name or id]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.ObjTarget == null)
			{
				invoker.WriteLine("Who do you wish to view the roles of?");
				return;
			}
			else if(!cmd.HasArgs(1))
			{
				invoker.WriteLine("Which role do you wish to add?");
				return;
			}

			PlayerAccount? acct = Program.Game.Repos.Accounts.FindAccount(cmd.ObjTarget);
			if(acct == null)
			{
				invoker.WriteLine($"Cannot find account for '{cmd.ObjTarget}'.");
				return;
			}

			GameRole? role = Program.Game.Mods.Roles.GetRole(cmd.StrArgs[0]);
			if(role == null)
			{
				invoker.WriteLine($"Cannot find role for '{cmd.StrArgs[0]}'.");
			}
			else
			{
				List<GameRole>? roles = acct.GetValue<List<GameRole>>(Field.Roles);
				if(roles == null)
				{
					roles = new List<GameRole>();
					acct.SetValue<List<GameRole>>(Field.Roles, roles);
				}
				if(!roles.Contains(role))
				{
					invoker.WriteLine($"They do not have that role.", true);
				}
				else
				{
					roles.Remove(role);
					Program.Game.Repos.Accounts.QueueForUpdate(acct);
					invoker.WriteLine($"Removed role '{role.name}' from '{acct.GetValue<string>(Field.Name)}'.");
				}
			}
		}
	}
}