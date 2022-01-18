using System.Collections.Generic;

namespace inspiral
{
	internal class CommandAddRole : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("addrole");
			Description = "Adds a role to an account.";
			Usage = "addrole [account name or id] [role name or id]";			
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.ObjTarget == null)
			{
				invoker.WriteLine("Who do you wish to view the roles of?", true);
				return;
			}
			else if(!cmd.HasArgs(1))
			{
				invoker.WriteLine("Which role do you wish to add?", true);
				return;
			}

			PlayerAccount? acct = Program.Game.Repos.Accounts.FindAccount(cmd.ObjTarget);
			if(acct == null)
			{
				invoker.WriteLine($"Cannot find account for '{cmd.ObjTarget}'.", true);
				return;
			}

			GameRole? role = Program.Game.Mods.Roles.GetRole(cmd.StrArgs[0].ToLower());
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
				if(roles.Contains(role))
				{
					invoker.WriteLine($"They already have that role.");
				}
				else
				{
					roles.Add(role);
					Program.Game.Repos.Accounts.QueueForUpdate(acct);
					invoker.WriteLine($"Added role '{role.name}' to '{acct.GetValue<string>(Field.Name)}'.");
				}
			}
		}
	}
}