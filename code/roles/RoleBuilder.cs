using System;
using System.Collections.Generic;

namespace inspiral
{
	internal class RoleBuilder : GameRole
	{
		internal RoleBuilder()
		{
			name = "Builder";
			AddCommand(Commands.Test);
			AddCommand(Commands.Create);
			AddCommand(Commands.Addroom);
			AddCommand(Commands.Set);
			AddCommand(Commands.View);
		}
	}
}