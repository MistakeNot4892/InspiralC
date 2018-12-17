using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace inspiral
{
	class ContextGeneral : GameContext
	{
		internal override void OnContextSet(GameClient viewer)
		{
			viewer.WriteLine($"Welcome, {viewer.shell.name}.");
			if(viewer.shell.location == null)
			{
				if(Modules.Components.Rooms.Count <= 0)
				{
					Debug.WriteLine("First run: cannot find a room, creating a new one.");
					Modules.Templates.Instantiate("room");
				}
				GameObject room = (GameObject)Modules.Components.Rooms[0].parent;
				viewer.shell.Move(room);
			}
			else
			{
				viewer.shell.location.ExaminedBy(viewer, true);
			}
		}
		internal override bool TakeInput(GameClient invoker, string command, string rawCommand, string arguments)
		{
			if(invoker.shell != null && invoker.shell.location != null && invoker.shell.location.HasComponent(Text.CompRoom) && invoker.shell.HasComponent(Text.CompMobile))
			{
				MobileComponent mob = (MobileComponent)invoker.shell.GetComponent(Text.CompMobile);
				if(mob.CanMove())
				{
					string tmp = command;
					if(Text.shortExits.ContainsKey(tmp))
					{
						tmp = Text.shortExits[tmp];
					}
					RoomComponent room = (RoomComponent)invoker.shell.location.GetComponent(Text.CompRoom);
					if(room.exits.ContainsKey(tmp))
					{
						GameObject destination = (GameObject)Game.Objects.Get(room.exits[tmp]);
						if(destination == null)
						{
							invoker.WriteLine($"Strangely, there is nothing to the {tmp}. You stay where you are.");
						}
						else
						{
							GameObject loc = invoker.shell.location;
							invoker.WriteLine($"You depart to the {tmp}.");
							destination.ShowToContents($"{mob.GetString(Text.FieldEnterMessage).Replace("$DIR", tmp)}");
							invoker.shell.Move(destination);
							loc.ShowToContents($"{mob.GetString(Text.FieldLeaveMessage).Replace("$DIR", tmp)}");
						}
						return true;
					}
				}
			}
			return InvokeCommand(invoker, command, arguments);
		}
		internal override string GetPrompt(GameClient viewer) 
		{
			string p = "";
			foreach(KeyValuePair<string, GameComponent> comp in viewer.shell.components)
			{
				string addP = comp.Value.GetPrompt();
				if(addP != null && addP != "")
				{
					p += $" {addP}";
				}
			}
			p = p.Trim();
			string final = Colours.Fg("> ", Colours.Yellow);
			if(p == null || p == "" || p == viewer.lastPrompt)
			{
				return final;
			}
			viewer.lastPrompt = p;
			return $"{p}{final}";
		}
	}
}