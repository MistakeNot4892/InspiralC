namespace inspiral
{
	internal class CommandStrike : GameCommand
	{

		internal override void Initialize()
		{
			Aliases.Add("strike");
			Aliases.Add("attack");
			Aliases.Add("hit");
			Aliases.Add("bash");
			Description = "Attacks another entity.";
			Usage = "strike [target] <in bodypart> <with bodypart or object>";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{

			GameObject? targetObj = null;
			if(cmd.ObjTarget != null && cmd.ObjTarget != "")
			{
				targetObj = invoker.FindGameObjectNearby(cmd.ObjTarget);
			}
			if(targetObj == null)
			{
				invoker.WriteLine($"You cannot find '{cmd.ObjTarget}' nearby.");

				return;
			}

			string? usingItem = cmd.ObjWith;
			GameObject? strikeWith = null;
			GameObject? strikeAgainst = null;

			var invComp = invoker.GetComponent<InventoryComponent>();
			if(invComp != null)
			{
				InventoryComponent inv = (InventoryComponent)invComp;
				if(usingItem == null || usingItem == "")
				{
					foreach(string slot in inv.GetWieldableSlots())
					{
						if(inv.carrying.ContainsKey(slot))
						{
							usingItem = $"{inv.carrying[slot].GetValue<long>(Field.Id)}";
							break;
						}
					}
				}
				else if(inv.GetWieldableSlots().Contains(usingItem) && inv.carrying.ContainsKey(usingItem))
				{
					usingItem = $"{inv.carrying[usingItem].GetValue<long>(Field.Id)}";
				}
			}

			var mobComp = invoker.GetComponent<MobileComponent>();
			if(mobComp != null)
			{
				MobileComponent mob = (MobileComponent)mobComp;
				if(strikeWith == null && (usingItem == null || usingItem == "") && mob.strikers.Count > 0)
				{
					usingItem = mob.strikers[Program.Game.Random.Next(0, mob.strikers.Count)];
				}
				if(usingItem != null && mob.strikers.Contains(usingItem))
				{
					strikeWith = mob.limbs[usingItem];
				}
			}

			if(strikeWith == null)
			{

				GameObject? prop = null;
				if(usingItem != null)
				{
					prop = invoker.FindGameObjectInContents(usingItem);
				}
				if(prop != null && invComp != null)
				{
					InventoryComponent inv = (InventoryComponent)invComp;
					if(inv.IsWielded(prop))
					{
						if(!prop.HasComponent<PhysicsComponent>())
						{
							invoker.WriteLine($"You cannot use {prop.GetShortDesc()} as a weapon.");
							return;
						}
						strikeWith = prop;
					}
				}
			}

			if(strikeWith == null)
			{
				invoker.WriteLine($"You are not holding '{usingItem}'.");
				return;
			}

			string strikeString = $"{strikeWith.GetShortDesc()}";
			var tarMobComp = targetObj.GetComponent<MobileComponent>();
			if(tarMobComp != null)
			{
				MobileComponent mob = (MobileComponent)tarMobComp;
				string? checkBp = cmd.ObjIn;
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
					invoker.WriteLine($"{Text.Capitalize(targetObj.GetShortDesc())} is missing that bodypart!");
					return;
				}
			}

			if(strikeAgainst != null)
			{
				strikeString = $"{strikeString}, {strikeAgainst.HandleImpact(invoker, strikeWith, 3.0)}";
			}
			string firstPersonStrikeWith = strikeString;
			if(firstPersonStrikeWith.Substring(0,2) == "a ")
			{
				firstPersonStrikeWith = firstPersonStrikeWith.Substring(2);
			}
			else if(firstPersonStrikeWith.Substring(0,3) == "an ")
			{
				firstPersonStrikeWith = firstPersonStrikeWith.Substring(3);
			}
			var bpComp = strikeWith.GetComponent<BodypartComponent>();
			if(bpComp != null)
			{
				BodypartComponent body = (BodypartComponent)bpComp;
				if(body.GetValue<bool>(Field.NaturalWeapon))
				{
					GenderObject genderObj = Program.Game.Mods.Gender.GetByTerm(invoker.GetValue<string>(Field.Gender));
					strikeString = $"{genderObj.Their} {strikeString}";
				}
			}

			string bpString = "";
			if(strikeAgainst != null)
			{
				bpString = $" in the {strikeAgainst.GetShortDesc()}";
			}
			invoker.ShowNearby(invoker, targetObj,
				$"You strike {targetObj.GetShortDesc()}{bpString} with your {firstPersonStrikeWith}!",
				$"{Text.Capitalize(invoker.GetShortDesc())} strikes you{bpString} with {strikeString}!",
				$"{Text.Capitalize(invoker.GetShortDesc())} strikes {targetObj.GetShortDesc()}{bpString} with {strikeString}!"
				);
		}
	}
}