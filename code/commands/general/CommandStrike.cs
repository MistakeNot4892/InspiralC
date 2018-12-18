using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{

		internal void CmdStrike(GameClient invoker, string invocation)
		{

			Tuple<string, string, string> tokens = FindTokensFromInput(invocation);

			GameObject targetObj = null;
			if(tokens.Item1 != null && tokens.Item1 != "")
			{
				targetObj = invoker.shell.FindGameObjectNearby(tokens.Item1);
			}
			if(targetObj == null)
			{
				invoker.shell.WriteLine($"You cannot find '{tokens.Item1}' nearby.");
				invoker.SendPrompt();
				return;
			}

			string strikeWith = null;
			string usingItem = tokens.Item3;
			if(invoker.shell.HasComponent(Text.CompInventory))
			{
				InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Text.CompInventory);
				if(usingItem == null || usingItem == "")
				{
					foreach(string slot in inv.GetWieldableSlots())
					{
						if(inv.carrying.ContainsKey(slot))
						{
							usingItem = inv.carrying[slot].id.ToString();
							break;
						}
					}
				}
				else if(inv.GetWieldableSlots().Contains(usingItem))
				{
					if(inv.carrying.ContainsKey(usingItem))
					{
						usingItem = inv.carrying[usingItem].id.ToString();
					}
					else
					{
						invoker.shell.WriteLine($"You are not holding anything in your {usingItem}!");
						invoker.SendPrompt();
						return;
					}
				}
			}

			if(strikeWith == null && invoker.shell.HasComponent(Text.CompMobile))
			{
				MobileComponent mob = (MobileComponent)invoker.shell.GetComponent(Text.CompMobile);
				if((usingItem == null || usingItem == "") && mob.bodyplan.strikers.Count > 0)
				{
					usingItem = mob.bodyplan.strikers[Game.rand.Next(0, mob.bodyplan.strikers.Count)];
				}
				if(mob.bodyplan.strikers.Contains(usingItem))
				{
					strikeWith = $"your {usingItem}";
				}
			}

			if(strikeWith == null)
			{
				GameObject prop = invoker.shell.FindGameObjectInContents(usingItem);
				if(prop != null && invoker.shell.HasComponent(Text.CompInventory))
				{
					InventoryComponent inv = (InventoryComponent)invoker.shell.GetComponent(Text.CompInventory);
					if(inv.IsWielded(prop))
					{
						strikeWith = $"{prop.GetString(Text.CompVisible, Text.FieldShortDesc)}";
					}
				}
			}

			if(strikeWith == null)
			{
				invoker.shell.WriteLine($"You are not holding '{usingItem}'.");
				invoker.SendPrompt();
				return;
			}

			string bpString = "";
			if(targetObj.HasComponent(Text.CompMobile))
			{
				MobileComponent mob = (MobileComponent)targetObj.GetComponent(Text.CompMobile);
				string checkBp = tokens.Item2;
				if(checkBp == null || checkBp == "")
				{
					checkBp = mob.GetWeightedRandomBodypart();
				}
				if(mob.bodyData.ContainsKey(checkBp))
				{
					bpString = $" in the {checkBp}";
				}
				if(bpString == "")
				{
					invoker.shell.WriteLine($"{Text.Capitalize(targetObj.GetString(Text.CompVisible, Text.FieldShortDesc))} is missing that bodypart!");
					invoker.SendPrompt();
					return;
				}
			}

			invoker.shell.WriteLine($"You strike {targetObj.GetString(Text.CompVisible, Text.FieldShortDesc)}{bpString} with {strikeWith}!");
			invoker.SendPrompt();
		}
	}
}