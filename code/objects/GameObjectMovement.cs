using System;
using System.Collections.Generic;

namespace inspiral
{
	internal partial class GameObject
	{
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
			ClientComponent clientComp = (ClientComponent)entering.GetComponent(Text.CompClient);
			if(clientComp != null && clientComp.client != null)
			{
				ExaminedBy(clientComp.client, true);
			}
			return true;
		}
	}
}