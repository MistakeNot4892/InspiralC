using System;
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
					client.client.SendPrompt();
				}
			}
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
		internal void Probed(GameClient invoker)
		{
			string reply = $"{GetString(Text.CompVisible, Text.FieldShortDesc)} ({name}#{id})";
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
		internal List<string> GetVisibleContents(GameClient viewer, bool quickView)
		{
			List<string> result = new List<string>();
			if(HasComponent(Text.CompMobile))
			{
				if(HasComponent(Text.CompInventory))
				{
					string their = (this == viewer.shell) ? "your" : gender.His;
					InventoryComponent equip = (InventoryComponent)GetComponent(Text.CompInventory);
					foreach(KeyValuePair<string, GameObject> equ in equip.carrying)
					{
						if(quickView)
						{
							if(equip.GetWieldableSlots().Contains(equ.Key))
							{
								result.Add($"{equ.Value.GetString(Text.CompVisible, Text.FieldShortDesc)} ({equ.Value.name}#{equ.Value.id}) ({equ.Key}, wielded)");
							}
							else
							{
								result.Add($"{equ.Value.GetString(Text.CompVisible, Text.FieldShortDesc)} ({equ.Value.name}#{equ.Value.id}) ({equ.Key}, worn)");
							}
						}
						else
						{
							if(equip.GetWieldableSlots().Contains(equ.Key))
							{
								result.Add($"{equ.Value.GetString(Text.CompVisible, Text.FieldShortDesc)} in {their} {equ.Key}.");
							}
							else
							{
								result.Add($"{equ.Value.GetString(Text.CompVisible, Text.FieldShortDesc)} on {their} {equ.Key}.");
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
						result.Add($"{gameObj.GetString(Text.CompVisible, Text.FieldShortDesc)} ({gameObj.name}#{gameObj.id})");
					}
					else
					{
						if(gameObj != viewer.shell)
						{
							result.Add(gameObj.GetString(Text.CompVisible, Text.FieldRoomDesc));
						}
					}
				}
			}
			return result;
		}
		internal void ExaminedBy(GameClient viewer, bool fromInside)
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
	}
}