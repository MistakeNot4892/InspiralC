namespace inspiral
{
	internal class CommandTakeRole : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "takerole" };
			description = "Removes a role from an account.";
			usage = "takerole [account name or id] [role name or id]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.objTarget == null)
			{
				invoker.SendLine("Who do you wish to view the roles of?");
				return;
			}
			else if(cmd.strArgs.Length < 1)
			{
				invoker.SendLine("Which role do you wish to add?");
				return;
			}

			PlayerAccount acct = Game.Accounts.FindAccount(cmd.objTarget);
			if(acct == null)
			{
				invoker.SendLine($"Cannot find account for '{cmd.objTarget}'.");
				return;
			}

			GameRole role = Modules.Roles.GetRole(cmd.strArgs[0]);
			if(role == null)
			{
				invoker.SendLine($"Cannot find role for '{cmd.strArgs[0]}'.");
			}
			else if(!acct.roles.Contains(role))
			{
				invoker.WriteLine($"They do not have that role.", true);
			}
			else
			{
				acct.roles.Remove(role);
				Game.Accounts.QueueForUpdate(acct);
				invoker.SendLine($"Removed role '{role.name}' from '{acct.userName}'.");
			}
		}
	}
}