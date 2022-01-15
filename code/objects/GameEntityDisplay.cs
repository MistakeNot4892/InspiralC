using System.Collections.Generic;

namespace inspiral
{
	internal partial class GameObject
	{
		internal void ShowToContents(string message)
		{
			foreach(GameObject obj in contents)
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
			foreach(GameObject obj in contents)
			{
				if(obj != source && obj.HasComponent<ClientComponent>())
				{
					obj.WriteLine(message3p, true);
				}
			}
		}

		internal virtual void WriteLine(string message)
		{
			WriteLine(message, false);
		}
		internal virtual void WriteLine(string message, bool sendPrompt)
		{
			ClientComponent client = (ClientComponent)GetComponent<ClientComponent>();
			if(client != null && client.client != null)
			{
				client.client.WriteLine(message);
				if(sendPrompt)
				{
					SendPrompt();
				}
			}
		}

		internal virtual void ShowNearby(GameObject source, GameObject other,
			string message1p, string message2p, string message3p)
		{
			source.WriteLine(message1p);
			other.WriteLine(message2p);
			ShowNearby(source, message3p, new List<GameObject>() { source, other });
		}

		internal virtual void ShowNearby(GameObject source, string message, List<GameObject> exceptions)
		{
			if(source.location != null)
			{
				foreach(GameObject obj in source.location.contents)
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

		internal virtual void ShowNearby(GameObject source, string message)
		{
			ShowNearby(source, message, message);
		}

		internal virtual void ShowNearby(GameObject source, string message1p, string message3p)
		{
			if(source.location != null)
			{
				source.location.ShowToContents(this, message1p, message3p);
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
			string reply = $"{GetShortDesc()} ({name}#{id})";
			reply += "\nContents:";
			if(contents.Count > 0)
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
				if(HasComponent<InventoryComponent>())
				{
					string their = (this == viewer) ? "your" : gender.Their;
					InventoryComponent equip = (InventoryComponent)GetComponent<InventoryComponent>();
					foreach(KeyValuePair<string, GameObject> equ in equip.carrying)
					{
						if(quickView)
						{
							if(equip.GetWieldableSlots().Contains(equ.Key))
							{
								result.Add($"{equ.Value.GetShortDesc()} ({equ.Value.name}#{equ.Value.id}) ({equ.Key}, wielded)");
							}
							else
							{
								result.Add($"{equ.Value.GetShortDesc()} ({equ.Value.name}#{equ.Value.id}) ({equ.Key}, worn)");
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
				foreach(GameObject gameObj in contents)
				{
					if(quickView)
					{
						result.Add($"{gameObj.GetShortDesc()} ({gameObj.name}#{gameObj.id})");
					}
					else
					{
						if(gameObj != viewer)
						{
							result.Add(gameObj.GetRoomDesc());
						}
					}
				}
			}
			return result;
		}
		internal void ExaminedBy(GameObject viewer, bool fromInside)
		{
			if(HasComponent<VisibleComponent>())
			{
				VisibleComponent comp = (VisibleComponent)GetComponent<VisibleComponent>();
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
			return GetString<VisibleComponent>(Text.FieldShortDesc);
		}
		internal string GetRoomDesc()
		{
			return ApplyStringTokens(GetString<VisibleComponent>(Text.FieldRoomDesc));
		}
		internal string GetColour(string colourType)
		{
			// TODO: client config
			return GlobalConfig.GetColour(colourType);
		}
	}
}