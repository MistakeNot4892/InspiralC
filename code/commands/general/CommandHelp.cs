using System.Collections.Generic;

namespace inspiral
{
	internal class CommandHelp : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("help");
			Description = "Shows details on commands. WIP.";
			Usage = "help <term>";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.RawInput != "" && cmd.RawInput != null)
			{
				invoker.WriteLine($"If the help system existed, you'd be searching for '{cmd.RawInput}'.");
			}
			else
			{
				invoker.WriteLine("Available commands:");
				PlayerAccount? account = invoker.GetAccount();
				if(account != null)
				{
					List<GameRole>? roles = account.GetValue<List<GameRole>>(Field.Roles);
					if(roles != null)
					{
						foreach(GameRole role in roles)
						{
							invoker.WriteLine(role.GetHelp());
						}
					}
				}
				invoker.WriteLine("\nEnd of command list.");
			}
		}
	}
}