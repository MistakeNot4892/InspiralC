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
		internal override string Name { get; set; } = "Player";
		internal override string Description { get; set; } = "Grants general world interaction and communication commands.";
		internal override List<GameCommand> UniqueCommands { get; set; } = new List<GameCommand>() {
			Commands.Say,
			Commands.Emote,
			Commands.Take,
			Commands.Drop,
			Commands.Inv,
			Commands.Quit,
			Commands.Look,
			Commands.Client,
			Commands.Help,
			Commands.Config
		};
	}
}