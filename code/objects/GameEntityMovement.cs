namespace inspiral
{
	internal partial class GameEntity
	{
		internal bool Move(GameEntity destination)
		{
			bool canMove = true;
			GameEntity lastLocation = location;
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
		internal bool OnDeparture(GameEntity departing)
		{
			return true;
		}
		internal bool OnEntry(GameEntity entering)
		{
			return true;
		}
		internal bool Exited(GameEntity leaving)
		{
			if(!contents.Contains(leaving) || !leaving.OnDeparture(this))
			{
				return false;
			}
			contents.Remove(leaving);
			return true;
		}
		internal bool Entered(GameEntity entering)
		{
			if(contents.Contains(entering) || !entering.OnEntry(this))
			{
				return false;
			}
			contents.Add(entering);
			if(entering.HasComponent<ClientComponent>())
			{
				ExaminedBy(entering, true);
				entering.SendPrompt();
			}
			return true;
		}
	}
}