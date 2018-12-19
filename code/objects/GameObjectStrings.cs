using System;
using System.Collections.Generic;

namespace inspiral
{
	internal partial class GameObject
	{
		internal string GetString(string component, string field)
		{
			if(components.ContainsKey(component))
			{
				return components[component].GetString(field);
			}
			return null;
		}
		internal long GetLong(string component, string field)
		{
			if(components.ContainsKey(component))
			{
				return components[component].GetLong(field);
			}
			return -1;
		}
		internal void SetLong(string component, string field, long newField)
		{
			if((bool)(GetComponent(component)?.SetValue(field, newField)))
			{
				Game.Objects.QueueForUpdate(this);
			}
		}
		internal void SetString(string component, string field, string newField)
		{
			if(HasComponent(component))
			{
				GameComponent comp = GetComponent(component);
				if(comp.SetValue(field, newField))
				{
					Game.Objects.QueueForUpdate(this);
				}
			}
		}

		internal string GetStringSummary(int wrapWidth)
		{
			Dictionary<string, List<string>> summary = new Dictionary<string, List<string>>();

			string fieldKey = $"Object summary for {name} (#{id})";
			summary.Add(fieldKey, new List<string>());
			summary[fieldKey].Add($"aliases:  {Text.EnglishList(aliases)}");
			summary[fieldKey].Add($"gender:   {gender.Term}");
			if(location != null)
			{
				summary[fieldKey].Add($"location (read-only): {location.GetShort()} (#{location.id})");
			}
			else
			{
				summary[fieldKey].Add($"location (read-only): null");
			}
			foreach(KeyValuePair<string, GameComponent> comp in components)
			{
				summary[fieldKey].Add($"\n{Text.FormatPopup(comp.Value.name, comp.Value.GetStringSummary(), wrapWidth+Text.NestedWrapwidthModifier)}");
			}
			return Text.FormatBlock(summary, wrapWidth);
		}
		internal string GetStringSummary(GameClient invoker)
		{
			return GetStringSummary(invoker.config.wrapwidth);
		}
	}
}