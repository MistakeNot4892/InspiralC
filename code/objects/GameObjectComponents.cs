using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

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
		internal GameComponent AddComponent(JProperty componentData)
		{
			GameComponent comp = null;
			try
			{
				comp = AddComponent(componentData.Name);
				comp.ConfigureFromJson(componentData.Value);
			}
			catch(Exception e)
			{
				Debug.WriteLine($"Exception when configuring component from JSON - {e.Message}");
			}
			return comp;
		} 

		internal GameComponent AddComponent(string componentKey) 
		{
			GameComponent component = null;
			if(HasComponent(componentKey))
			{
				component = GetComponent(componentKey);
			}
			else
			{
				component = Modules.Components.MakeComponent(componentKey);
				component.name = componentKey;
				components.Add(componentKey, component);
				component.Added(this);
			}
			return component;
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