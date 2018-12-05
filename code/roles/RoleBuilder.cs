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