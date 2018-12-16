using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal static partial class Command
	{
		internal static void CmdTest(GameClient invoker, string invocation)
		{
			GameObject hat = (GameObject)Game.Objects.CreateNewInstance(true);
			hat.name = "tophat";

			hat.AddComponent(Components.Wearable);
			WearableComponent wear = (WearableComponent)hat.GetComponent(Components.Wearable);
			wear.wearableSlots.Add("head");

			hat.AddComponent(Components.Visible);
			hat.SetString(Components.Visible, Text.FieldShortDesc,    "a tophat");
			hat.SetString(Components.Visible, Text.FieldRoomDesc,     "A tophat has been misplaced here.");
			hat.SetString(Components.Visible, Text.FieldExaminedDesc, "It's a very generic tophat. You get the impression it only exists for the sake of debugging.");

			hat.Move(invoker.shell.location);
			invoker.WriteLine($"Created {hat.GetString(Components.Visible, Text.FieldShortDesc)} ({hat.name}#{hat.id}).");
			invoker.SendPrompt();
		}
	}
}