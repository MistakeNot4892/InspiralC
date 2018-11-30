using System;
using System.Collections.Generic;

namespace inspiral
{
	class GameObject
	{
		internal long id;
		internal long templateId;
		internal Dictionary<string, string> strings;
		internal List<GameObject> contents;
		internal GameObject location;

		internal GameObject()
		{
			contents = new List<GameObject>();
			strings = new Dictionary<string, string>();
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
				destination.Entered(this);
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
			if(entering.HasClient())
			{
				ExaminedBy(entering.GetClient(), true);
			}
			return true;
		}

		internal void SetString(string field, string newString)
		{
			strings.Remove(field);
			strings.Add(field, newString);
		}
		internal string GetString(string field)
		{
			if(strings.ContainsKey(field))
			{
				return strings[field];
			}
			GameObjectTemplate gameTemplate = (GameObjectTemplate)Game.Templates.Get(templateId);
			if(gameTemplate != null)
			{
				return gameTemplate.GetString(field);
			}
			return $"?{field}?";
		}
		internal void ExaminedBy(GameClient viewer, bool fromInside)
		{
			string mainDesc = $"{Colours.Fg(Text.Capitalize(GetString(Text.FieldShortDesc)),Colours.BoldWhite)}.\n{Colours.Fg(GetString(Text.FieldExaminedDesc), Colours.BoldBlack)}";
			if(contents.Count > 0)
			{
				List<string> roomAppearances = new List<string>();
				foreach(GameObject obj in contents)
				{
					if(obj != viewer.currentGameObject)
					{
						roomAppearances.Add(obj.GetRoomAppearance());
					}
				}
				if(roomAppearances.Count > 0)
				{
					mainDesc = $"{mainDesc}\n{string.Join(" ", roomAppearances.ToArray())}";
				}
			}
			viewer.WriteLinePrompted(mainDesc);
		}

		internal virtual string GetRoomAppearance()
		{
			return GetString(Text.FieldRoomDesc);
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
			if(HasClient())
			{
				GetClient().WriteLinePrompted(message);
			}
		}

		internal virtual bool HasClient()
		{
			return false;
		}

		internal virtual GameClient GetClient()
		{
			return null;
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
		internal virtual void Logout() {}
		internal virtual void Login(GameClient _client) {}
	}
}
