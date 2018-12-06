using System;
using System.Collections.Generic;

namespace inspiral
{
	internal partial class GameObject
	{
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
				component.name = componentKey;
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
	}
}