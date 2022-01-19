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
			string fieldKey = $"Object summary for {GetValue<string>(Field.Name)} (#{GetValue<ulong>(Field.Id)})";
			summary.Add(fieldKey, new List<string>());


			foreach(KeyValuePair<DatabaseField, object> field in Fields)
			{
				if(!field.Key.fieldIsViewable) 
				{
					continue;
				}
				if(field.Key.fieldIsReference)
				{
					summary[fieldKey].Add($"reference field {field.Key.fieldName} skipped");
					continue;
				}
				string fieldSummary = field.Key.fieldName;
				if(!field.Key.fieldIsEditable)
				{
					fieldSummary = $"{fieldSummary} (read-only)";
				}
				summary[fieldKey].Add($"{fieldSummary}: {field.Value.ToString()}");
			}

			foreach(KeyValuePair<System.Type, GameComponent> comp in Components)
			{
				string compSummary = comp.Value.GetStringSummary();
				if(compSummary != "")
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