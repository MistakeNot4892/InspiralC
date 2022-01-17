namespace inspiral
{
	internal class CommandExits : GameCommand
	{
		internal override void Initialize()
		{
			Aliases = new System.Collections.Generic.List<string>() { "exits" };
			Description = "Shows the exits of the current location.";
			Usage = "exits";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(invoker.Location == null || !invoker.Location.HasComponent<RoomComponent>())
			{
				invoker.WriteLine("You cannot see any exits here."); 
				return;
			}
			RoomComponent roomComp = (RoomComponent)invoker.Location.GetComponent<RoomComponent>();
			invoker.WriteLine($"{Colours.Fg(roomComp.GetExitString(), invoker.GetColour(Text.ColourDefaultExits))}"); 
		}
	}
}



