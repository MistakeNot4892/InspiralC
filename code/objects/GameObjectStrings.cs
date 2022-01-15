using System.Collections.Generic;

namespace inspiral
{
	internal partial class GameObject
	{
		internal string ApplyStringTokens(string msg)
		{
			if(HasComponent<VisibleComponent>())
			{
				string shortDesc = GetShortDesc();
				msg = msg.Replace(GlobalConfig.StringTokenShort, shortDesc);
				msg = msg.Replace(GlobalConfig.StringTokenShortCaps, Text.Capitalize(shortDesc));
			}
			return msg;
		}

		internal string ApplyStringTokens(string msg, string dir)
		{
			msg = msg.Replace(GlobalConfig.StringTokenDir, dir);
			return ApplyStringTokens(msg);
		}
		internal string GetStringSummary(GameObject viewer, int wrapWidth)
		{
			Dictionary<string, List<string>> summary = new Dictionary<string, List<string>>();
			GenderObject genderObj = Modules.Gender.GetByTerm(GetValue<string>(Field.Gender));
			string fieldKey = $"Object summary for {GetValue<string>(Field.Name)} (#{GetValue<long>(Field.Id)})";
			summary.Add(fieldKey, new List<string>());
			summary[fieldKey].Add($"aliases:  {Text.EnglishList(GetValue<List<string>>(Field.Aliases))}");
			summary[fieldKey].Add($"gender:   {genderObj.Term}");

			if(Location != null)
			{
				summary[fieldKey].Add($"location (read-only): {Location.GetShortDesc()} (#{Location.GetValue<long>(Field.Id)})");
			}
			else
			{
				summary[fieldKey].Add($"location (read-only): null");
			}
			foreach(KeyValuePair<System.Type, GameComponent> comp in Components)
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