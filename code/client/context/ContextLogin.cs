using System.Collections.Generic;

namespace inspiral
{
	class ContextLogin : GameContext
	{
		internal override void OnContextSet(GameClient viewer)
		{
			viewer.WriteLine($"{GameColours.Fg("    =========", "red")}{GameColours.Fg(" Welcome to Inspiral, Coalescence, Ringdown ", "boldred")}{GameColours.Fg("=========", "red")}");
			viewer.WriteLine(GameColours.Fg("\n         - Enter a username to log in.", "white"));
			viewer.WriteLine($"{GameColours.Fg("         - Enter ","white")}{GameColours.Fg("register", "boldwhite")}{GameColours.Fg(" to register a new account.", "white")}");
			viewer.WriteLine(GameColours.Fg("\n    ==============================================================", "red"));
		}

		internal override bool TakeInput(GameClient invoker, string command, string arguments)
		{
			invoker.clientId = GameText.Capitalize(command);
			invoker.currentGameObject.SetString("short_description", GameText.Capitalize(command));
			invoker.currentGameObject.SetString("room_description", $"{invoker.currentGameObject.GetString("short_description")} is here.");
			invoker.SetContext(new ContextGeneral());
			return true;
		}
	}
}