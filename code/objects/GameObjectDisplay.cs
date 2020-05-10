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
			if(source.HasComponent(Text.CompClient))
			{
				source.WriteLine(message1p, sendPromptToSource);
			}
			foreach(GameObject obj in contents)
			{
				if(obj != source && obj.HasComponent(Text.CompClient))
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
			ClientComponent client = (ClientComponent)GetComponent(Text.CompClient);
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
			source.SendLine(message1p);
			other.SendLine(message2p);
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
				if(source.HasComponent(Text.CompClient))
				{
					source.WriteLine(message1p);
				}
			}
		}
		internal void Probed(GameObject invoker)
		{
			string reply = $"{GetShort()} ({name}#{id})";
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
			invoker.SendLine(reply);
		}

		internal void SendLine(string input)
		{
			WriteLine(input, true);
		}
		internal List<string> GetVisibleContents(GameObject viewer, bool quickView)
		{
			List<string> result = new List<string>();
			if(HasComponent(Text.CompMobile))
			{
				if(HasComponent(Text.CompInventory))
				{
					string their = (this == viewer) ? "your" : gender.Their;
					InventoryComponent equip = (InventoryComponent)GetComponent(Text.CompInventory);
					foreach(KeyValuePair<string, GameObject> equ in equip.carrying)
					{
						if(quickView)
						{
							if(equip.GetWieldableSlots().Contains(equ.Key))
							{
								result.Add($"{equ.Value.GetShort()} ({equ.Value.name}#{equ.Value.id}) ({equ.Key}, wielded)");
							}
							else
							{
								result.Add($"{equ.Value.GetShort()} ({equ.Value.name}#{equ.Value.id}) ({equ.Key}, worn)");
							}
						}
						else
						{
							if(equip.GetWieldableSlots().Contains(equ.Key))
							{
								result.Add($"{equ.Value.GetShort()} in {their} {equ.Key}.");
							}
							else
							{
								result.Add($"{equ.Value.GetShort()} on {their} {equ.Key}.");
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
						result.Add($"{gameObj.GetShort()} ({gameObj.name}#{gameObj.id})");
					}
					else
					{
						if(gameObj != viewer)
						{
							result.Add(gameObj.GetString(Text.CompVisible, Text.FieldRoomDesc));
						}
					}
				}
			}
			return result;
		}
		internal void ExaminedBy(GameObject viewer, bool fromInside)
		{
			if(HasComponent(Text.CompVisible))
			{
				VisibleComponent comp = (VisibleComponent)GetComponent(Text.CompVisible);
				comp.ExaminedBy(viewer, fromInside);
			}
			else
			{
				viewer.WriteLine("There is nothing there.");
			}
		}
		internal string GetShort()
		{
			return GetString(Text.CompVisible, Text.FieldShortDesc);
		}
	}
}