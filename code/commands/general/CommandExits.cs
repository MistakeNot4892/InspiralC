namespace inspiral
{
	internal class CommandExits : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("exits");
			Description = "Shows the exits of the current location.";
			Usage = "exits";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			var getRoomComp = invoker.Location?.GetComponent<RoomComponent>();
			if(getRoomComp == null)
			{
				invoker.WriteLine("You cannot see any exits here."); 
				return;
			}
			RoomComponent roomComp = (RoomComponent)getRoomComp;
			invoker.WriteLine($"{Colours.Fg(roomComp.GetExitString(), invoker.GetColour(Text.ColourDefaultExits))}"); 
		}
	}
}



