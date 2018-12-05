using System;
using System.Collections.Generic;

namespace inspiral
{
	class GameObject
	{
		internal long id;
		internal string name = "object";
		internal long gender = Gender.Inanimate;
		internal List<string> aliases = new List<string>();
		internal GameObject location;
		internal List<GameObject> contents;
		internal long flags = 0;
		internal Dictionary<string, GameComponent> components;
		internal GameObject()
		{
			contents = new List<GameObject>();
			components = new Dictionary<string, GameComponent>();
		}
		internal GameComponent GetComponent(string componentKey) 
		{
			if(HasComponent(componentKey))
			{
				return components[componentKey];
			}
			return null;
		}
		internal void AddComponent(string componentKey) 
		{
			if(!HasComponent(componentKey))
			{
				GameComponent component = Components.MakeComponent(componentKey);
				components.Add(componentKey, component);
				component.Added(this);
			}
		}
		internal void RemoveComponent(string componentKey)
		{
			if(HasComponent(componentKey))
			{
				GameComponent component = components[componentKey];
				components.Remove(componentKey);
				component.Removed(this);
			}
		}
		internal bool HasComponent(string componentKey)
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
				viewer.SendLineWithPrompt("There is nothing there.");
			}
		}
		internal string GetString(string component, string field)
		{
			if(components.ContainsKey(component))
			{
				return components[component].GetString(field);
			}
			return null;
		}
		internal long GetLong(string component, string field)
		{
			if(components.ContainsKey(component))
			{
				return components[component].GetLong(field);
			}
			return -1;
		}
		internal void SetLong(string component, string field, long newField)
		{
			if((bool)(GetComponent(component)?.SetValue(field, newField)))
			{
				Game.Objects.QueueForUpdate(this);
			}
		}
		internal void SetString(string component, string field, string newField)
		{
			if((bool)(GetComponent(component)?.SetValue(field, newField)))
			{
				Game.Objects.QueueForUpdate(this);
			}
		}
		internal GameObject FindGameObjectNearby(string token)
		{
			string checkToken = token.ToLower();
			if(checkToken == "me" || checkToken == "self")
			{
				return this;
			}
			if(location != null)
			{
				if(checkToken == "here" || 
					checkToken == "room" || 
					"{location.id}" == checkToken || 
					location.name.ToLower() == checkToken || 
					location.aliases.Contains(checkToken)
					)
				{
					return location;
				}
				return location.FindGameObjectInContents(checkToken);
			}
			return null;
		}
		internal void EditValue(GameClient editor, string field, string value)
		{
			string lastVal = "";
			switch(field.ToLower())
			{
				case "name":
					lastVal = name;
					name = value;
					break;
				case "short":
					lastVal = GetString(Components.Visible, Text.FieldShortDesc);
					SetString(Components.Visible, Text.FieldShortDesc, value);
					break;
				case "room":
					lastVal = GetString(Components.Visible, Text.FieldRoomDesc);
					SetString(Components.Visible, Text.FieldRoomDesc, value);
					break;
				case "examined":
					lastVal = GetString(Components.Visible, Text.FieldExaminedDesc);
					SetString(Components.Visible, Text.FieldExaminedDesc, value);
					break;
				case "enter":
					lastVal = GetString(Components.Mobile, Text.FieldEnterMessage);
					SetString(Components.Mobile, Text.FieldEnterMessage, value);
					break;
				case "leave":
					lastVal = GetString(Components.Mobile, Text.FieldLeaveMessage);
					SetString(Components.Mobile, Text.FieldLeaveMessage, value);
					break;
				case "dead":
					lastVal = GetString(Components.Mobile, Text.FieldDeathMessage);
					SetString(Components.Mobile, Text.FieldDeathMessage, value);
					break;
				default:
					editor.SendLineWithPrompt("Unknown field. Valid fields are: name, short, room, examined, enter, leave, dead.");
					return;
			}
			editor.SendLineWithPrompt($"Set field '{field}' of object #{id} ({GetString(Components.Visible, Text.FieldShortDesc)}) to '{value}'.\nFor reference, previous value was '{lastVal}'.");
		}
		internal GameObject FindGameObjectInContents(string token)
		{
			string checkToken = token.ToLower();
			foreach(GameObject gameObj in contents)
			{
				if($"{gameObj.id}" == checkToken || 
					gameObj.name.ToLower() == checkToken || 
					gameObj.aliases.Contains(checkToken)
					)
				{
					return gameObj;
				}
			}

			if(HasComponent(Components.Room))
			{
				RoomComponent room = (RoomComponent)GetComponent(Components.Room);
				string lookingFor = checkToken;
				if(Text.shortExits.ContainsKey(lookingFor))
				{
					lookingFor = Text.shortExits[lookingFor];
				}
				if(room.exits.ContainsKey(lookingFor))
				{
					GameObject otherRoom = (GameObject)Game.Objects.Get(room.exits[lookingFor]);
					if(otherRoom != null)
					{
						return otherRoom;
					}
				}
			}
			return null;
		}
		internal bool Move(GameObject destination)
		{
			bool canMove = true;
			GameObject lastLocation = location;
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
			if(location != lastLocation)
			{
				Game.Objects.QueueForUpdate(this);
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
		internal string GetStringSummary(GameClient invoker)
		{
			Dictionary<string, List<string>> summary = new Dictionary<string, List<string>>();

			string fieldKey = $"Object summary for {name} (id #{id})";
			summary.Add(fieldKey, new List<string>());
			summary[fieldKey].Add($"Aliases:  {Text.EnglishList(aliases)}");
			summary[fieldKey].Add($"Gender:   {Gender.Term(gender)} ({gender})");
			if(location != null)
			{
				summary[fieldKey].Add($"Location: {location.id}");
			}
			else
			{
				summary[fieldKey].Add($"Location: null");
			}
			foreach(KeyValuePair<string, GameComponent> comp in components)
			{
				summary[fieldKey].Add($"\n{Text.FormatPopup(comp.Value.name, comp.Value.GetStringSummary(), invoker.config.wrapwidth-13)}");
			}
			return Text.FormatBlock(summary, invoker.config.wrapwidth);
		}
	}
}
