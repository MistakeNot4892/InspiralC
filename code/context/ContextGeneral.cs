using System.Collections.Generic;

namespace inspiral
{
	class ContextGeneral : GameContext
	{
		internal override void OnContextSet(GameClient viewer)
		{
			viewer.WriteLine($"Welcome, {viewer.shell.GetValue<string>(Field.Name)}.");
			if(viewer.shell.Location == null)
			{
				viewer.shell.Move(Game.Modules.Rooms.GetSpawnRoom());
			}
			else
			{
				viewer.shell.Location.ExaminedBy(viewer.shell, true);
				viewer.SendPrompt();
			}
		}
		internal override bool TakeInput(GameClient invoker, string command, string rawCommand, string arguments)
		{

			var mobComp = invoker.shell.GetComponent<MobileComponent>();
			var roomComp = invoker.shell.Location?.GetComponent<RoomComponent>();

			if(invoker.shell != null && invoker.shell.Location != null && roomComp != null && mobComp != null)
			{
				MobileComponent mob = (MobileComponent)mobComp;
				if(mob.CanMove())
				{
					string tmp = command;
					if(Text.shortExits.ContainsKey(tmp))
					{
						tmp = Text.shortExits[tmp];
					}
					RoomComponent room = (RoomComponent)roomComp;
					if(room.exits.ContainsKey(tmp))
					{
						var destObj = Game.Repositories.Objects.GetById(room.exits[tmp]);
						if(destObj == null)
						{
							invoker.WriteLine($"Strangely, there is nothing to the {tmp}. You stay where you are.");
						}
						else
						{
							GameObject destination = (GameObject)destObj;
							GameObject loc = invoker.shell.Location;
							invoker.WriteLine($"You depart to the {tmp}.");
							string? enterMsg = mob.GetValue<string>(Field.EnterMessage);
							if(enterMsg != null)
							{
								destination.ShowToContents(invoker.shell.ApplyStringTokens(enterMsg, tmp));
							}
							invoker.shell.Move(destination);
							string? leaveMsg = mob.GetValue<string>(Field.LeaveMessage);
							if(leaveMsg != null)
							{
								loc.ShowToContents(invoker.shell.ApplyStringTokens(leaveMsg, tmp));
							}
						}
						invoker.SendPrompt();
						return true;
					}
				}
			}
			return InvokeCommand(invoker, command, arguments);
		}
		internal override string? GetPrompt(GameClient viewer) 
		{
			if(viewer.shell == null)
			{
				return null;
			}
			string p = "";
			foreach(KeyValuePair<System.Type, GameComponent> comp in viewer.shell.Components)
			{
				string addP = comp.Value.GetPrompt();
				if(addP != null && addP != "")
				{
					p += $" {addP}";
				}
			}
			p = p.Trim();
			string final = Colours.Fg("> ", viewer.shell.GetColour(Text.ColourDefaultPrompt));
			if(p == null || p == "" || p == viewer.lastPrompt)
			{
				return final;
			}
			viewer.lastPrompt = p;
			return $"{p}{final}";
		}
	}
}