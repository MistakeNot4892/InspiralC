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
	}
}