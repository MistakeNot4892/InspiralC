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
		internal override string Name { get; set; } = "Builder";
		internal override string Description { get; set; } = "Grants room and object viewing/modification functions.";
		internal override List<GameCommand> UniqueCommands { get; set; } = new List<GameCommand>() {
			Commands.Create,
			Commands.Addroom,
			Commands.Set,
			Commands.View
		};
	}
}