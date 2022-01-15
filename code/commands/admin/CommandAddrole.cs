namespace inspiral
{
	internal class CommandAddRole : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "addrole" };
			description = "Adds a role to an account.";
			usage = "addrole [account name or id] [role name or id]";			
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.objTarget == null)
			{
				invoker.WriteLine("Who do you wish to view the roles of?", true);
				return;
			}
			else if(cmd.strArgs.Length < 1)
			{
				invoker.WriteLine("Which role do you wish to add?", true);
				return;
			}

			PlayerAccount acct = Game.Accounts.FindAccount(cmd.objTarget);
			if(acct == null)
			{
				invoker.WriteLine($"Cannot find account for '{cmd.objTarget}'.", true);
				return;
			}

			GameRole role = Modules.Roles.GetRole(cmd.strArgs[0].ToLower());
			if(role == null)
			{
				invoker.WriteLine($"Cannot find role for '{cmd.strArgs[0]}'.");
			}
			else if(acct.roles.Contains(role))
			{
				invoker.WriteLine($"They already have that role.");
			}
			else
			{
				acct.roles.Add(role);
				Game.Accounts.QueueForUpdate(acct);
				invoker.WriteLine($"Added role '{role.name}' to '{acct.userName}'.");
			}
		}
	}
}