using System;
using System.Collections.Generic;

namespace inspiral
{
	class GameObject
	{
		internal GameObject location;
		internal List<GameObject> contents;
		internal long id;
		internal string name = "object";
		internal long flags = 0;
		internal List<string> aliases = new List<string>();
		internal Dictionary<int, GameComponent> components;
		internal GameObject()
		{
			contents = new List<GameObject>();
			components = new Dictionary<int, GameComponent>();
		}

		internal GameComponent GetComponent(int componentKey) 
		{
			if(HasComponent(componentKey))
			{
				return components[componentKey];
			}
			return null;
		}
		internal void AddComponent(int componentKey) 
		{
			if(!HasComponent(componentKey))
			{
				GameComponent component = Components.MakeComponent(componentKey);
				components.Add(componentKey, component);
				component.Added(this);
			}
		}
		internal void RemoveComponent(int componentKey)
		{
			if(HasComponent(componentKey))
			{
				GameComponent component = components[componentKey];
				components.Remove(componentKey);
				component.Removed(this);
			}
		}

		internal bool HasComponent(int componentKey)
		{
			return components.ContainsKey(componentKey);
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
				viewer.WriteLinePrompted("There is nothing there.");
			}
		}
		internal string GetString(int component, int field)
		{
			return GetComponent(component)?.GetStringValue(field) ?? null;
		}
		internal long GetLong(int component, int field)
		{
			return GetComponent(component)?.GetLongValue(field) ?? 0;
		}
		internal void SetLong(int component, int field, long newField)
		{
			GetComponent(component)?.SetValue(field, newField);
		}
		internal void SetString(int component, int field, string newField)
		{
			GetComponent(component)?.SetValue(field, newField);
		}
		internal GameObject FindInContents(string token)
		{
			string checkToken = token.ToLower();
			foreach(GameObject gameObj in contents)
			{
				if($"{gameObj.id}" == checkToken || gameObj.name.ToLower() == checkToken || gameObj.aliases.Contains(checkToken))
				{
					return gameObj;
				}
			}
			return null;
		}











		internal bool Move(GameObject destination)
		{
			bool canMove = true;
			if(location != null)
			{
				canMove = location.Exited(this);
			}
			if(canMove)
			{
				location = destination;
				if(destination != null)
				{
					destination.Entered(this);
				}
			}
			return canMove;
		}

		internal bool OnDeparture(GameObject departing)
		{
			return true;
		}

		internal bool OnEntry(GameObject entering)
		{
			return true;
		}

		internal bool Exited(GameObject leaving)
		{
			if(!contents.Contains(leaving) || !leaving.OnDeparture(this))
			{
				return false;
			}
			contents.Remove(leaving);
			return true;
		}

		internal bool Entered(GameObject entering)
		{
			if(contents.Contains(entering) || !entering.OnEntry(this))
			{
				return false;
			}
			contents.Add(entering);
			ClientComponent clientComp = (ClientComponent)entering.GetComponent(Components.Client);
			if(clientComp != null && clientComp.client != null)
			{
				ExaminedBy(clientComp.client, true);
			}
			return true;
		}

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
			source.ShowMessage(message1p);
			foreach(GameObject obj in contents)
			{
				if(obj != source)
				{
					obj.ShowMessage(message3p);
				}
			}
		}
		internal virtual void ShowMessage(string message)
		{
			ClientComponent clientComp = (ClientComponent)GetComponent(Components.Client);
			if(clientComp != null && clientComp.client != null)
			{
				clientComp.client.WriteLinePrompted(message);
			}
		}

		internal virtual void ShowNearby(GameObject source, string message1p, string message3p)
		{
			if(source.location != null)
			{
				source.location.ShowToContents(this, message1p, message3p);
			}
			else
			{
				source.ShowMessage(message1p);
			}
		}
	}
}
