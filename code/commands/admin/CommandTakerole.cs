namespace inspiral
{
	internal class CommandTakeRole : GameCommand
	{
		internal override void Initialize()
		{
			Aliases = new System.Collections.Generic.List<string>() { "takerole" };
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
			else if(cmd.StrArgs.Length < 1)
			{
				invoker.WriteLine("Which role do you wish to add?");
				return;
			}

			PlayerAccount acct = Repos.Accounts.FindAccount(cmd.ObjTarget);
			if(acct == null)
			{
				invoker.WriteLine($"Cannot find account for '{cmd.ObjTarget}'.");
				return;
			}

			GameRole role = Modules.Roles.GetRole(cmd.StrArgs[0]);
			if(role == null)
			{
				invoker.WriteLine($"Cannot find role for '{cmd.StrArgs[0]}'.");
			}
			else if(!acct.roles.Contains(role))
			{
				invoker.WriteLine($"They do not have that role.", true);
			}
			else
			{
				acct.roles.Remove(role);
				Repos.Accounts.QueueForUpdate(acct);
				invoker.WriteLine($"Removed role '{role.name}' from '{acct.userName}'.");
			}
		}
	}
}