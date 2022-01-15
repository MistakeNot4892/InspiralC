using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class GameObject : GameEntity
	{
		internal GameComponent GetComponent<T>() 
		{
			return GetComponent(typeof(T));
		}
		internal GameComponent GetComponent(System.Type compType) 
		{
			if(HasComponent(compType))
			{
				return components[compType];
			}
			return null;
		}

		internal GameComponent AddComponent(System.Type compType)
		{
			GameComponent comp = GetComponent(compType);
			if(comp == null)
			{
				comp = Modules.Components.MakeComponent(compType);
				components.Add(compType, comp);
				comp.Added(this);
			}
			return comp;
		}
		internal GameComponent AddComponent<T>()
		{
			return AddComponent(typeof(T));
		}
		internal void RemoveComponent<T>()
		{
			if(HasComponent<T>())
			{
				GameComponent component = components[typeof(T)];
				components.Remove(typeof(T));
				component.Removed(this);
			}
		}
		internal bool HasComponent(System.Type compType)
		{
			return components.ContainsKey(compType);
		}
		internal bool HasComponent<T>()
		{
			return HasComponent(typeof(T));
		}
	}
}