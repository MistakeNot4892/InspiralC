using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Roles
	{
		internal static RoleAdministrator Administrator = new RoleAdministrator();
	}
	internal class RoleAdministrator : GameRole
	{
		internal override string Description { get; set; } = "Contains role and game administration functions.";
		internal RoleAdministrator()
		{
			name = "Administrator";
			AddCommand(Commands.Test);
			AddCommand(Commands.Roles);
			AddCommand(Commands.Addrole);
			AddCommand(Commands.Takerole);
		}
	}
}