using System.Collections.Generic;

namespace inspiral
{
	internal partial class GameObject
	{
		internal void ShowToContents(string message)
		{
			foreach(GameObject obj in Contents)
			{
				obj.WriteLine(message, true);
			}
		}
		internal void ShowToContents(GameObject source, string message)
		{
			ShowToContents(source, message, message);
		}
		internal void ShowToContents(GameObject source, string message1p, string message3p)
		{
			ShowToContents(source, message1p, message3p, false);
		}

		internal void ShowToContents(GameObject source, string message1p, string message3p, bool sendPromptToSource)
		{
			if(source.HasComponent<ClientComponent>())
			{
				source.WriteLine(message1p, sendPromptToSource);
			}
			foreach(GameObject obj in Contents)
			{
				if(obj != source && obj.HasComponent<ClientComponent>())
				{
					obj.WriteLine(message3p, true);
				}
			}
		}
		internal void WriteLine(string message)
		{
			WriteLine(message, false);
		}
		internal void WriteLine(string message, bool sendPrompt)
		{
			var clientComp = GetComponent<ClientComponent>();
			if(clientComp != null)
			{
				ClientComponent client = (ClientComponent)clientComp;
				if(client.client != null)
				{
					client.client.WriteLine(message);
					if(sendPrompt)
					{
						SendPrompt();
					}
				}
			}
		}

		internal void ShowNearby(GameObject source, GameObject other,
			string message1p, string message2p, string message3p)
		{
			source.WriteLine(message1p);
			other.WriteLine(message2p);
			ShowNearby(source, message3p, new List<GameObject>() { source, other });
		}

		internal void ShowNearby(GameObject source, string message, List<GameObject> exceptions)
		{
			GameObject? sourceLocation = source.Location;
			if(sourceLocation != null)
			{
				foreach(GameObject obj in sourceLocation.Contents)
				{
					if(!exceptions.Contains(obj))
					{
						obj.WriteLine(message, true);

					}
				}
			}
			else
			{
				if(!exceptions.Contains(source))
				{
					source.WriteLine(message, true);
				}
			}
		}

		internal void ShowNearby(GameObject source, string message)
		{
			ShowNearby(source, message, message);
		}

		internal void ShowNearby(GameObject source, string message1p, string message3p)
		{
			GameObject? sourceLocation = source.Location;
			if(sourceLocation != null)
			{
				sourceLocation.ShowToContents(this, message1p, message3p);
			}
			else
			{
				if(source.HasComponent<ClientComponent>())
				{
					source.WriteLine(message1p);
				}
			}
		}
		internal void Probed(GameObject invoker)
		{
			string reply = $"{GetShortDesc()} ({GetValue<string>(Field.Name)}#{GetValue<long>(Field.Id)})";
			reply += "\nContents:";
			if(Contents.Count > 0)
			{
				foreach(string visibleThing in GetVisibleContents(invoker, true))
				{
					reply += $"\n- {visibleThing}.";
				}
			}
			else
			{
				reply += "\n- nothing.";
			}
			invoker.WriteLine(reply);
		}

		internal List<string> GetVisibleContents(GameObject viewer, bool quickView)
		{
			List<string> result = new List<string>();
			if(HasComponent<MobileComponent>())
			{
				var invComp = GetComponent<InventoryComponent>();
				if(invComp != null)
				{
					GenderObject genderObj = Modules.Gender.GetByTerm(GetValue<string>(Field.Gender));
					string their = (this == viewer) ? "your" : genderObj.Their;
					InventoryComponent equip = (InventoryComponent)invComp;
					foreach(KeyValuePair<string, GameObject> equ in equip.carrying)
					{
						if(quickView)
						{
							if(equip.GetWieldableSlots().Contains(equ.Key))
							{
								result.Add($"{equ.Value.GetShortDesc()} ({equ.Value.GetValue<string>(Field.Name)}#{equ.Value.GetValue<long>(Field.Id)}) ({equ.Key}, wielded)");
							}
							else
							{
								result.Add($"{equ.Value.GetShortDesc()} ({equ.Value.GetValue<string>(Field.Name)}#{equ.Value.GetValue<long>(Field.Id)}) ({equ.Key}, worn)");
							}
						}
						else
						{
							if(equip.GetWieldableSlots().Contains(equ.Key))
							{
								result.Add($"{equ.Value.GetShortDesc()} in {their} {equ.Key}.");
							}
							else
							{
								result.Add($"{equ.Value.GetShortDesc()} on {their} {equ.Key}.");
							}
						}
					}
				}
			}
			else
			{
				foreach(GameObject gameObj in Contents)
				{
					if(quickView)
					{
						result.Add($"{gameObj.GetShortDesc()} ({gameObj.GetValue<string>(Field.Name)}#{gameObj.GetValue<long>(Field.Id)})");
					}
					else
					{
						if(gameObj != viewer)
						{
							string? roomDesc = gameObj.GetRoomDesc();
							if(roomDesc != null)
							{
								result.Add(roomDesc);
							}
						}
					}
				}
			}
			return result;
		}
		internal void ExaminedBy(GameObject viewer, bool fromInside)
		{
			var visComp = GetComponent<VisibleComponent>();
			if(visComp != null)
			{
				VisibleComponent comp = (VisibleComponent)visComp;
				comp.ExaminedBy(viewer, fromInside);
			}
			else
			{
				viewer.WriteLine("There is nothing there.");
			}
		}
		internal string GetShortDesc()
		{
			// Re-enable token replacement if relevant tokens are added
			string? ret = GetValue<string>(Field.ShortDesc);
			if(ret == null)
			{
				return "unknown";
			}
			return ret;
		}
		internal string? GetRoomDesc()
		{
			string? roomDesc = GetValue<string>(Field.RoomDesc);
			if(roomDesc != null)
			{
				return ApplyStringTokens(roomDesc);
			}
			return null;
		}
		internal string GetColour(string colourType)
		{
			// TODO: client config
			return GlobalConfig.GetColour(colourType);
		}
	}
}