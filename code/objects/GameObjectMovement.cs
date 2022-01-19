namespace inspiral
{
	internal partial class GameObject
	{
		internal bool Move(GameObject destination)
		{
			bool canMove = true;
			GameObject? lastLocation = Location;
			if(lastLocation != null)
			{
				canMove = lastLocation.Exited(this);
			}
			if(canMove)
			{
				SetValue<ulong>(Field.Location, destination.GetValue<ulong>(Field.Id));
				if(destination != null)
				{
					destination.Entered(this);
				}
			}
			if(Location != lastLocation)
			{
				Repositories.Objects.QueueForUpdate(this);
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
			if(!Contents.Contains(leaving) || !leaving.OnDeparture(this))
			{
				return false;
			}
			Contents.Remove(leaving);
			return true;
		}
		internal bool Entered(GameObject entering)
		{
			if(Contents.Contains(entering) || !entering.OnEntry(this))
			{
				return false;
			}
			Contents.Add(entering);
			if(entering.HasComponent<ClientComponent>())
			{
				ExaminedBy(entering, true);
				entering.SendPrompt();
			}
			return true;
		}
	}
}