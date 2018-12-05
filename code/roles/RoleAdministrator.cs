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
		internal override string Name { get; set; } = "Administrator";
		internal override List<GameCommand> UniqueCommands { get; set; } = new List<GameCommand>() {
			Commands.Test, 
			Commands.Roles, 
			Commands.Addrole, 
			Commands.Takerole
		};
		internal override string Description { get; set; } = "Contains role and game administration functions.";
	}
}