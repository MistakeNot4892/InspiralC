using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{

		internal void CmdStrike(GameObject invoker, CommandData cmd)
		{

			GameObject targetObj = null;
			if(cmd.objTarget != null && cmd.objTarget != "")
			{
				targetObj = invoker.FindGameObjectNearby(cmd.objTarget);
			}
			if(targetObj == null)
			{
				invoker.WriteLine($"You cannot find '{cmd.objTarget}' nearby.");
				invoker.SendPrompt();
				return;
			}

			bool naturalWeapon = false;
			string strikeWith = null;
			string usingItem = cmd.objWith;
			if(invoker.HasComponent(Text.CompInventory))
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent(Text.CompInventory);
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
						invoker.WriteLine($"You are not holding anything in your {usingItem}!");
						invoker.SendPrompt();
						return;
					}
				}
			}

			if(strikeWith == null && invoker.HasComponent(Text.CompMobile))
			{
				MobileComponent mob = (MobileComponent)invoker.GetComponent(Text.CompMobile);
				if((usingItem == null || usingItem == "") && mob.bodyplan.strikers.Count > 0)
				{
					usingItem = mob.bodyplan.strikers[Game.rand.Next(0, mob.bodyplan.strikers.Count)];
				}
				if(mob.bodyplan.strikers.Contains(usingItem))
				{
					strikeWith = $"{usingItem}";
					naturalWeapon = true;
				}
			}

			if(strikeWith == null)
			{
				GameObject prop = invoker.FindGameObjectInContents(usingItem);
				if(prop != null && invoker.HasComponent(Text.CompInventory))
				{
					InventoryComponent inv = (InventoryComponent)invoker.GetComponent(Text.CompInventory);
					if(inv.IsWielded(prop))
					{
						strikeWith = $"{prop.GetShort()}";
					}
				}
			}

			if(strikeWith == null)
			{
				invoker.WriteLine($"You are not holding '{usingItem}'.");
				invoker.SendPrompt();
				return;
			}

			string bpString = "";
			if(targetObj.HasComponent(Text.CompMobile))
			{
				MobileComponent mob = (MobileComponent)targetObj.GetComponent(Text.CompMobile);
				string checkBp = cmd.objIn;
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
					invoker.WriteLine($"{Text.Capitalize(targetObj.GetShort())} is missing that bodypart!");
					invoker.SendPrompt();
					return;
				}
			}

			if(naturalWeapon)
			{
				invoker.ShowNearby(invoker, targetObj,
					$"You strike {targetObj.GetShort()}{bpString} with your {strikeWith}!",
					$"{Text.Capitalize(invoker.GetShort())} strikes you{bpString} with {invoker.gender.His} {strikeWith}!",
					$"{Text.Capitalize(invoker.GetShort())} strikes {targetObj.GetShort()}{bpString} with {invoker.gender.His} {strikeWith}!"
					);
			}
			else
			{
				invoker.ShowNearby(invoker, targetObj,
					$"You strike {targetObj.GetShort()}{bpString} with {strikeWith}!",
					$"{Text.Capitalize(invoker.GetShort())} strikes you{bpString} with {strikeWith}!",
					$"{Text.Capitalize(invoker.GetShort())} strikes {targetObj.GetShort()}{bpString} with {strikeWith}!"
					);
			}
		}
	}
}