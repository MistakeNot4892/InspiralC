using System.Collections.Generic;

namespace inspiral
{
	class ContextGeneral : GameContext
	{
		internal override void Initialize() 
		{
			AddCommand(new CommandSay());
			AddCommand(new CommandEmote());
			AddCommand(new CommandQuit());
		}
	}
}