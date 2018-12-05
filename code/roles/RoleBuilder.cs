using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Roles
	{
		internal static RoleBuilder Builder = new RoleBuilder();
	}
	internal class RoleBuilder : GameRole
	{
		internal override string Description { get; set; } = "Grants room and object viewing/modification functions.";
		internal RoleBuilder()
		{
			name = "Builder";
			AddCommand(Commands.Create);
			AddCommand(Commands.Addroom);
			AddCommand(Commands.Set);
			AddCommand(Commands.View);
		}
	}
}