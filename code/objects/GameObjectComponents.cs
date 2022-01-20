namespace inspiral
{
	internal partial class GameObject : IGameEntity
	{
		internal bool HasComponent(System.Type compType)
		{
			return Components.ContainsKey(compType);
		}
		internal bool HasComponent<T>()
		{
			return HasComponent(typeof(T));
		}
		internal GameComponent? GetComponent<T>() 
		{
			return GetComponent(typeof(T));
		}
		internal GameComponent? GetComponent(System.Type compType) 
		{
			if(HasComponent(compType))
			{
				return Components[compType];
			}
			return null;
		}

		internal GameComponent? AddComponent(System.Type compType)
		{
			GameComponent? comp = GetComponent(compType);
			if(comp == null)
			{
				comp = Repositories.Components.MakeComponent(compType);
				if(comp != null)
				{
					Components.Add(compType, comp);
					comp.Added(this);
				}
			}
			return comp;
		}
		internal GameComponent? AddComponent<T>()
		{
			return AddComponent(typeof(T));
		}
		internal void RemoveComponent<T>()
		{
			if(HasComponent<T>())
			{
				GameComponent? component = GetComponent<T>();
				Components.Remove(typeof(T));
				if(component != null)
				{
					component.Removed(this);
				}
			}
		}
	}
}