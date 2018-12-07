using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal static class Bodyplans
	{
		internal static Dictionary<string, Bodyplan> plans = new Dictionary<string, Bodyplan>();
		internal static Dictionary<string, Bodypart> parts = new Dictionary<string, Bodypart>();
		static Bodyplans()
		{
			Debug.WriteLine("Loading bodypart definitions.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data\definitions\bodies\parts", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Debug.WriteLine($"Loading bodypart definition {f.File}.");
				try
				{
					JObject r = JObject.Parse(File.ReadAllText(f.File));
					Bodypart bp = new Bodypart((string)r["name"], (string)r["root"], (bool)r["grasp"], (bool)r["stance"], (bool)r["vital"], JsonConvert.DeserializeObject<List<string>>(r["slots"].ToString()));
					parts.Add(bp.name, bp);
				}
				catch(Exception e)
				{
					Debug.WriteLine($"Exception when loading bodypart from file {f.File} - {e.Message}");
				}
			}
			Debug.WriteLine("Done.\nLoading bodyplan definitions.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data\definitions\bodies\plans", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Debug.WriteLine($"Loading bodyplan definition {f.File}.");
				try
				{
					JObject r = JObject.Parse(File.ReadAllText(f.File));
					Bodyplan bPlan = new Bodyplan(r["name"].ToString().ToLower());
					foreach(JProperty token in r["parts"].Children<JProperty>())
					{
						Bodypart child = GetPart(token.Name.ToString());
						bPlan.allParts.Add(child);
						Bodypart parent = token.Value != null ? GetPart(token.Value.ToString()) : null;
						if(parent != null)
						{
							bPlan.childToParent.Add(child, parent);
							if(!bPlan.parentToChildren.ContainsKey(parent))
							{
								bPlan.parentToChildren.Add(parent, new List<Bodypart>());
							}
							bPlan.parentToChildren[parent].Add(child);
						}
					}
					bPlan.FinalizePlan();
					plans.Add(bPlan.name, bPlan);
				}
				catch(Exception e)
				{
					Debug.WriteLine($"Exception when loading bodyplan from file {f.File} - {e.Message}");
				}
			}
			Debug.WriteLine("Done.");
		}
		internal static Bodyplan GetPlan(string name)
		{
			name = name.ToLower();
			return plans.ContainsKey(name) ? plans[name] : null;
		}
		internal static Bodypart GetPart(string name)
		{
			name = name.ToLower();
			return parts.ContainsKey(name) ? parts[name] : null;
		}
	}

	internal class Bodyplan
	{
		internal string name;
		internal Bodypart root;
		internal List<Bodypart> allParts = new List<Bodypart>();
		internal Dictionary<Bodypart, Bodypart> childToParent = new Dictionary<Bodypart, Bodypart>();
		internal Dictionary<Bodypart, List<Bodypart>> parentToChildren = new Dictionary<Bodypart, List<Bodypart>>();
		internal List<Bodypart> graspers = new List<Bodypart>();
		internal List<Bodypart> stance = new List<Bodypart>();
		internal List<string> equipmentSlots = new List<string>();
		internal Bodyplan(string _name)
		{
			name = _name;
		}
		internal void FinalizePlan()
		{
			foreach(Bodypart bp in allParts)
			{
				if(bp.canGrasp && !graspers.Contains(bp))
				{
					graspers.Add(bp);
				}
				if(bp.canStand && !stance.Contains(bp))
				{
					stance.Add(bp);
				}
				foreach(string slot in bp.equipmentSlots)
				{
					if(!equipmentSlots.Contains(slot))
					{
						equipmentSlots.Add(slot);
					}
				}
			}
		}
		internal string GetSummary()
		{
			string summary = $"Bodyplan '{name}':";

			summary += "\n\nParts:";
			foreach(Bodypart bp in allParts)
			{
				summary += $"\n{bp.name} ";
				if(childToParent.ContainsKey(bp))
				{
					summary += $"- parent [{childToParent[bp].name}]";
				}
				if(parentToChildren.ContainsKey(bp))
				{
					summary += "- children [";
					foreach(Bodypart child in parentToChildren[bp])
					{
						summary += $" [{child.name}] ";
					}
					summary += "]";
				}
			}
			summary += "\n\nEquip slots:";
			if(equipmentSlots.Count > 0)
			{
				summary += $" {Text.EnglishList(equipmentSlots)}.";
			}
			else
			{
				summary += " none.";
			}
			summary += "\n\nGrasp:";
			if(graspers.Count > 0)
			{
				foreach(Bodypart grip in graspers)
				{
					summary += $" [{grip.name}]";
				}
			}
			else
			{
				summary += " none.";
			}
			summary += "\n\nStance:";
			if(stance.Count > 0)
			{
				foreach(Bodypart stand in stance)
				{
					summary += $" [{stand.name}]";
				}
			}
			else
			{
				summary += " none.";
			}
			return summary;
		}
	}

	internal class Bodypart
	{
		internal string name;
		internal string parent;
		internal bool canGrasp = false;
		internal bool canStand = false;
		internal bool isVital = false;
		internal List<string> equipmentSlots = new List<string>();
		internal Bodypart(string _name, string _parent, bool _grasp, bool _stance, bool _vital, List<string> _slots)
		{
			name = _name;
			parent = _parent;
			canGrasp = _grasp;
			canStand = _stance;
			isVital = _vital;
			equipmentSlots = _slots;
		}
	}
}