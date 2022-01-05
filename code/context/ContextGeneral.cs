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
					Game.LogError("First run: cannot find a room, creating a new one.");
					Modules.Templates.Instantiate("room");
				}
				GameEntity room = (GameEntity)Modules.Components.Rooms[0].parent;
				viewer.shell.Move(room);
			}
			else
			{
				viewer.shell.location.ExaminedBy(viewer.shell, true);
				viewer.SendPrompt();
			}
		}
		internal override bool TakeInput(GameClient invoker, string command, string rawCommand, string arguments)
		{
			if(invoker.shell != null && invoker.shell.location != null && invoker.shell.location.HasComponent<RoomComponent>() && invoker.shell.HasComponent<MobileComponent>())
			{
				MobileComponent mob = (MobileComponent)invoker.shell.GetComponent<MobileComponent>();
				if(mob.CanMove())
				{
					string tmp = command;
					if(Text.shortExits.ContainsKey(tmp))
					{
						tmp = Text.shortExits[tmp];
					}
					RoomComponent room = (RoomComponent)invoker.shell.location.GetComponent<RoomComponent>();
					if(room.exits.ContainsKey(tmp))
					{
						GameEntity destination = (GameEntity)Game.Objects.GetByID(room.exits[tmp]);
						if(destination == null)
						{
							invoker.WriteLine($"Strangely, there is nothing to the {tmp}. You stay where you are.");
						}
						else
						{
							GameEntity loc = invoker.shell.location;
							invoker.WriteLine($"You depart to the {tmp}.");
							destination.ShowToContents($"{mob.GetString(Text.FieldEnterMessage).Replace("$DIR", tmp)}");
							invoker.shell.Move(destination);
							loc.ShowToContents($"{mob.GetString(Text.FieldLeaveMessage).Replace("$DIR", tmp)}");
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
			foreach(KeyValuePair<System.Type, GameComponent> comp in viewer.shell.components)
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