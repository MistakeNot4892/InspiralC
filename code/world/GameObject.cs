using System;
using System.Collections.Generic;

namespace inspiral
{
	class GameObject
	{
		private long templateId;
		private Dictionary<string, string> strings;
		internal List<GameObject> contents;
		internal GameObject location;

		public GameObject()
		{
			contents = new List<GameObject>();
			strings = new Dictionary<string, string>();
		}

		public bool Move(GameObject destination)
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

		public bool OnDeparture(GameObject departing)
		{
			return true;
		}

		public bool OnEntry(GameObject entering)
		{
			return true;
		}

		public bool Exited(GameObject leaving)
		{
			if(!contents.Contains(leaving) || !leaving.OnDeparture(this))
			{
				return false;
			}
			contents.Remove(leaving);
			return true;
		}

		public bool Entered(GameObject entering)
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

		public void SetString(string field, string newString)
		{
			strings.Remove(field);
			strings.Add(field, newString);
		}
		public string GetString(string field)
		{
			if(strings.ContainsKey(field))
			{
				return strings[field];
			}
			GameObject gameObjTemplate = GameEntityRepository.GetObject(templateId);
			if(gameObjTemplate != null)
			{
				gameObjTemplate.GetString(field);
			}
			return $"unknown {field}";
		}
		public void ExaminedBy(GameClient viewer, bool fromInside)
		{
			string mainDesc = $"{GameColours.Fg(GetString("short_description"),"boldwhite")}\n{GameColours.Fg(GetString("examined_description"), "boldblack")}";
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

		public virtual string GetRoomAppearance()
		{
			return GetString("room_description");
		}

		public void ShowToContents(string message)
		{
			foreach(GameObject obj in contents)
			{
				obj.ShowMessage(message);
			}
		}

		public void ShowToContents(GameObject source, string message)
		{
			ShowToContents(source, message, message);
		}

		public void ShowToContents(GameObject source, string message1p, string message3p)
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
		public virtual void ShowMessage(string message)
		{
			if(HasClient())
			{
				GetClient().WriteLinePrompted(message);
			}
		}

		public virtual bool HasClient()
		{
			return false;
		}

		public virtual GameClient GetClient()
		{
			return null;
		}

		public virtual void ShowNearby(GameObject source, string message1p, string message3p)
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
		public virtual void Logout() {}
		public virtual void Login(GameClient _client) {}
	}
}
