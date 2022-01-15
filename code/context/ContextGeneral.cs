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
				viewer.shell.Move(Modules.Rooms.GetSpawnRoom());
			}
			else
			{
				viewer.shell.Location.ExaminedBy(viewer.shell, true);
				viewer.SendPrompt();
			}
		}
		internal override bool TakeInput(GameClient invoker, string command, string rawCommand, string arguments)
		{
			if(invoker.shell != null && invoker.shell.Location != null && invoker.shell.Location.HasComponent<RoomComponent>() && invoker.shell.HasComponent<MobileComponent>())
			{
				MobileComponent mob = (MobileComponent)invoker.shell.GetComponent<MobileComponent>();
				if(mob.CanMove())
				{
					string tmp = command;
					if(Text.shortExits.ContainsKey(tmp))
					{
						tmp = Text.shortExits[tmp];
					}
					RoomComponent room = (RoomComponent)invoker.shell.Location.GetComponent<RoomComponent>();
					if(room.exits.ContainsKey(tmp))
					{
						GameObject destination = (GameObject)Game.Objects.GetByID(room.exits[tmp]);
						if(destination == null)
						{
							invoker.WriteLine($"Strangely, there is nothing to the {tmp}. You stay where you are.");
						}
						else
						{
							GameObject loc = invoker.shell.Location;
							invoker.WriteLine($"You depart to the {tmp}.");
							destination.ShowToContents(invoker.shell.ApplyStringTokens(mob.GetValue<string>(Field.EnterMessage), tmp));
							invoker.shell.Move(destination);
							loc.ShowToContents(invoker.shell.ApplyStringTokens(mob.GetValue<string>(Field.LeaveMessage), tmp));
						}
						invoker.SendPrompt();
						return true;
					}
				}
			}
			return InvokeCommand(invoker, command, arguments);
		}
		internal override string GetPrompt(GameClient viewer) 
		{
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