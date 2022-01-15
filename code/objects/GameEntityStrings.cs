using System.Collections.Generic;

namespace inspiral
{
	internal partial class GameObject
	{
		internal string ApplyStringTokens(string msg)
		{
			if(HasComponent<VisibleComponent>())
			{
				msg = msg.Replace(GlobalConfig.StringTokenShort, GetString<VisibleComponent>(Field.ShortDesc));
				msg = msg.Replace(GlobalConfig.StringTokenShortCaps, Text.Capitalize(GetString<VisibleComponent>(Field.ShortDesc)));
			}
			return msg;
		}

		internal string ApplyStringTokens(string msg, string dir)
		{
			msg = msg.Replace(GlobalConfig.StringTokenDir, dir);
			return ApplyStringTokens(msg);
		}
		internal string GetString<T>(string field)
		{
			System.Type compType = typeof(T);
			if(components.ContainsKey(compType))
			{
				return components[compType].GetString(field);
			}
			return null;
		}
		internal long GetLong<T>(string field)
		{
			System.Type compType = typeof(T);
			if(components.ContainsKey(compType))
			{
				return components[compType].GetLong(field);
			}
			return -1;
		}
		internal void SetLong<T>(string field, long newField)
		{
			if((bool)(GetComponent<T>()?.SetValue(field, newField)))
			{
				Game.Objects.QueueForUpdate(this);
			}
		}
		internal void SetString<T>(string field, string newField)
		{
			if(HasComponent<T>())
			{
				GameComponent comp = GetComponent<T>();
				if(comp.SetValue(field, newField))
				{
					Game.Objects.QueueForUpdate(this);
				}
			}
		}

		internal string GetStringSummary(GameObject viewer, int wrapWidth)
		{
			Dictionary<string, List<string>> summary = new Dictionary<string, List<string>>();

			string fieldKey = $"Object summary for {name} (#{GetLong(Field.Id)})";
			summary.Add(fieldKey, new List<string>());
			summary[fieldKey].Add($"aliases:  {Text.EnglishList(aliases)}");
			summary[fieldKey].Add($"gender:   {gender.Term}");
			if(location != null)
			{
				summary[fieldKey].Add($"location (read-only): {location.GetShortDesc()} (#{location.GetLong(Field.Id)})");
			}
			else
			{
				summary[fieldKey].Add($"location (read-only): null");
			}
			foreach(KeyValuePair<System.Type, GameComponent> comp in components)
			{
				string compSummary = comp.Value.GetStringSummary();
				if(compSummary != null)
				{
					summary[fieldKey].Add($"\n{Text.FormatPopup(this, comp.Value.GetType().ToString(), compSummary, wrapWidth+Text.NestedWrapwidthModifier)}");
				}
			}
			return Text.FormatBlock(this, summary, wrapWidth);
		}
		internal string GetStringSummary(GameClient invoker)
		{
			return GetStringSummary(invoker.shell, invoker.config.wrapwidth);
		}
	}
}