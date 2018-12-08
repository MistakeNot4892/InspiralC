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
				obj.ShowMessage(message);
			}
		}
		internal void ShowToContents(GameObject source, string message)
		{
			ShowToContents(source, message, message);
		}
		internal void ShowToContents(GameObject source, string message1p, string message3p)
		{
			if(source.HasComponent(Components.Client))
			{
				source.ShowMessage(message1p);
			}
			foreach(GameObject obj in contents)
			{
				if(obj != source && obj.HasComponent(Components.Client))
				{
					obj.ShowMessage(message3p);
				}
			}
		}
		internal virtual void ShowMessage(string message)
		{
			ClientComponent client = (ClientComponent)GetComponent(Components.Client);
			if(client != null)
			{
				client.client?.SendLineWithPrompt(message);
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
						obj.ShowMessage(message);
					}
				}
			}
			else
			{
				if(!exceptions.Contains(source))
				{
					source.ShowMessage(message);
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
				if(source.HasComponent(Components.Client))
				{
					source.ShowMessage(message1p);
				}
			}
		}
		internal void Probed(GameClient invoker)
		{
			string reply = $"{GetString(Components.Visible, Text.FieldShortDesc)} ({name}#{id})";
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
			invoker.SendLineWithPrompt(reply);
		}
		internal List<string> GetVisibleContents(GameClient viewer, bool quickView)
		{
			List<string> result = new List<string>();
			if(HasComponent(Components.Mobile))
			{
				if(HasComponent(Components.Inventory))
				{
					InventoryComponent equip = (InventoryComponent)GetComponent(Components.Inventory);
					foreach(KeyValuePair<string, GameObject> equ in equip.carrying)
					{
						if(quickView)
						{
							result.Add($"{equ.Value.GetString(Components.Visible, Text.FieldShortDesc)} ({equ.Value.name}#{equ.Value.id}) ({equ.Key})");
						}
						else
						{
							result.Add($"{equ.Value.GetString(Components.Visible, Text.FieldShortDesc)} in {gender.His} {equ.Key}");
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
						result.Add($"{gameObj.GetString(Components.Visible, Text.FieldShortDesc)} ({gameObj.name}#{gameObj.id})");
					}
					else
					{
						if(gameObj != viewer.shell)
						{
							result.Add(gameObj.GetString(Components.Visible, Text.FieldRoomDesc));
						}
					}
				}
			}
			return result;
		}
		internal void ExaminedBy(GameClient viewer, bool fromInside)
		{
			if(HasComponent(Components.Visible))
			{
				VisibleComponent comp = (VisibleComponent)GetComponent(Components.Visible);
				comp.ExaminedBy(viewer, fromInside);
			}
			else
			{
				viewer.SendLineWithPrompt("There is nothing there.");
			}
		}
	}
}