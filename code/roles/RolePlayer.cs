using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Roles
	{
		internal static RolePlayer Player = new RolePlayer();
	}
	internal class RolePlayer : GameRole
	{
		internal override string Description { get; set; } = "Grants general world interaction and communication commands.";

		internal RolePlayer()
		{
			name = "Player";
			AddCommand(Commands.Say);
			AddCommand(Commands.Emote);
			AddCommand(Commands.Quit);
			AddCommand(Commands.Look);
			AddCommand(Commands.Client);
		}
	}
}