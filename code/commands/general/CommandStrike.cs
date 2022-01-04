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

			string usingItem = cmd.objWith;
			GameObject strikeWith = null;
			GameObject strikeAgainst = null;

			if(invoker.HasComponent<InventoryComponent>())
			{
				InventoryComponent inv = (InventoryComponent)invoker.GetComponent<InventoryComponent>();
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
				else if(inv.GetWieldableSlots().Contains(usingItem) && inv.carrying.ContainsKey(usingItem))
				{
					usingItem = inv.carrying[usingItem].id.ToString();
				}
			}

			if(invoker.HasComponent<MobileComponent>())
			{
				MobileComponent mob = (MobileComponent)invoker.GetComponent<MobileComponent>();
				if(strikeWith == null && (usingItem == null || usingItem == "") && mob.strikers.Count > 0)
				{
					usingItem = mob.strikers[Game.rand.Next(0, mob.strikers.Count)];
				}
				if(mob.strikers.Contains(usingItem))
				{
					strikeWith = mob.limbs[usingItem];
				}
			}

			if(strikeWith == null)
			{
				GameObject prop = invoker.FindGameObjectInContents(usingItem);
				if(prop != null && invoker.HasComponent<InventoryComponent>())
				{
					InventoryComponent inv = (InventoryComponent)invoker.GetComponent<InventoryComponent>();
					if(inv.IsWielded(prop))
					{
						if(!prop.HasComponent<PhysicsComponent>())
						{
							invoker.SendLine($"You cannot use {prop.GetShort()} as a weapon.");
							return;
						}
						strikeWith = prop;
					}
				}
			}

			if(strikeWith == null)
			{
				invoker.WriteLine($"You are not holding '{usingItem}'.");
				invoker.SendPrompt();
				return;
			}

			if(targetObj.HasComponent<MobileComponent>())
			{
				MobileComponent mob = (MobileComponent)targetObj.GetComponent<MobileComponent>();
				string checkBp = cmd.objIn;
				if(checkBp == null || checkBp == "")
				{
					checkBp = mob.GetWeightedRandomBodypart();
				}
				if(mob.limbs.ContainsKey(checkBp) && mob.limbs[checkBp] != null)
				{
					strikeAgainst = mob.limbs[checkBp];
				}
				else
				{
					invoker.WriteLine($"{Text.Capitalize(targetObj.GetShort())} is missing that bodypart!");
					invoker.SendPrompt();
					return;
				}
			}

			string strikeString = $"{strikeWith.GetShort()}, {strikeAgainst.HandleImpact(invoker, strikeWith, 3.0)}";
			string firstPersonStrikeWith = strikeString;
			if(firstPersonStrikeWith.Substring(0,2) == "a ")
			{
				firstPersonStrikeWith = firstPersonStrikeWith.Substring(2);
			}
			else if(firstPersonStrikeWith.Substring(0,3) == "an ")
			{
				firstPersonStrikeWith = firstPersonStrikeWith.Substring(3);
			}
			if(strikeWith.HasComponent<BodypartComponent>())
			{
				BodypartComponent body = (BodypartComponent)strikeWith.GetComponent<BodypartComponent>();
				if(body.isNaturalWeapon)
				{
					strikeString = $"{invoker.gender.Their} {strikeString}";
				}
			}

			string bpString = $" in the {strikeAgainst.GetShort()}";
			invoker.ShowNearby(invoker, targetObj,
				$"You strike {targetObj.GetShort()}{bpString} with your {firstPersonStrikeWith}!",
				$"{Text.Capitalize(invoker.GetShort())} strikes you{bpString} with {strikeString}!",
				$"{Text.Capitalize(invoker.GetShort())} strikes {targetObj.GetShort()}{bpString} with {strikeString}!"
				);
		}
	}
}