using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal static partial class Modules
	{
		internal static BodyModule Bodies;
	}
	internal class BodyModule : GameModule
	{
		internal Dictionary<string, Bodyplan> plans = new Dictionary<string, Bodyplan>();
		internal Dictionary<string, Bodypart> parts = new Dictionary<string, Bodypart>();
		internal override void Initialize() 
		{
			Modules.Bodies = this;
			Debug.WriteLine("- Loading bodypart definitions.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data/definitions/bodies/parts", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Debug.WriteLine($"- Loading bodypart definition {f.File}.");
				try
				{
					JObject r = JObject.Parse(File.ReadAllText(f.File));
					Bodypart bp = new Bodypart(
						(string)r["name"], 
						(string)r["root"], 
						 JsonConvert.DeserializeObject<List<string>>(r["slots"].ToString())
					);
					if(!JsonExtensions.IsNullOrEmpty(r["grasp"]))
					{
						bp.canGrasp = (bool)r["grasp"];
					}
					if(!JsonExtensions.IsNullOrEmpty(r["stance"]))
					{
						bp.canStand = (bool)r["stance"];
					}
					if(!JsonExtensions.IsNullOrEmpty(r["vital"]))
					{
						bp.isVital = (bool)r["vital"];
					}
					if(!JsonExtensions.IsNullOrEmpty(r["attackstrings"]))
					{
						bp.isNaturalWeapon = true;
						//bp.attackStrings = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(r["attackstrings"].ToString());
					}
					if(!JsonExtensions.IsNullOrEmpty(r["physics"]))
					{
						bp.contactData = JsonConvert.DeserializeObject<Dictionary<string, int>>(r["physics"].ToString());
					}
					parts.Add(bp.name, bp);
				}
				catch(Exception e)
				{
					Debug.WriteLine($"Exception when loading bodypart from file {f.File} - {e.Message}");
				}
			}
			Debug.WriteLine("Done.\nLoading bodyplan definitions.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data/definitions/bodies/plans", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Debug.WriteLine($"- Loading bodyplan definition {f.File}.");
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
					plans.Add(bPlan.name, bPlan);
				}
				catch(Exception e)
				{
					Debug.WriteLine($"Exception when loading bodyplan from file {f.File} - {e.Message}");
				}
			}
			Debug.WriteLine("Done.");
		}
		internal Bodyplan GetPlan(string name)		{
			name = name.ToLower();
			return plans.ContainsKey(name) ? plans[name] : null;
		}
		internal Bodypart GetPart(string name)
		{
			name = name.ToLower();
			return parts.ContainsKey(name) ? parts[name] : null;
		}
	}
	internal class Bodyplan
	{
		internal string name;
		internal List<Bodypart> allParts = new List<Bodypart>();
		internal Dictionary<Bodypart, Bodypart> childToParent = new Dictionary<Bodypart, Bodypart>();
		internal Dictionary<Bodypart, List<Bodypart>> parentToChildren = new Dictionary<Bodypart, List<Bodypart>>();
		internal Bodyplan(string _name)
		{
			name = _name;
		}
	}
	internal class Bodypart
	{
		internal string name;
		internal string parent;
		internal bool canGrasp = false;
		internal bool canStand = false;
		internal bool isVital = false;
		internal bool isNaturalWeapon = false;
		internal bool isEdged = false;
		internal long length = 10;
		internal long width = 10;
		internal long height = 10;
		internal double strikeArea = 10;
		internal Dictionary<string, int> contactData;
		internal List<string> equipmentSlots = new List<string>();
		internal List<Tuple<string, string>> attackStrings = new List<Tuple<string, string>>();

		internal Bodypart(string _name, string _parent, List<string> _slots)
		{
			name = _name;
			parent = _parent;
			equipmentSlots = _slots;
		}
	}
}