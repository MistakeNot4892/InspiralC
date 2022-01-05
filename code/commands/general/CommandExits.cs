namespace inspiral
{
	internal class CommandExits : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "exits" };
			description = "Shows the exits of the current location.";
			usage = "exits";
		}
		internal override void InvokeCommand(GameEntity invoker, CommandData cmd)
		{
			if(invoker.location == null || !invoker.location.HasComponent<RoomComponent>())
			{
				invoker.WriteLine("You cannot see any exits here."); 
				return;
			}
			RoomComponent roomComp = (RoomComponent)invoker.location.GetComponent<RoomComponent>();
			invoker.WriteLine($"{Colours.Fg(roomComp.GetExitString(), invoker.GetColour(Text.ColourDefaultExits))}"); 
		}
	}
}


