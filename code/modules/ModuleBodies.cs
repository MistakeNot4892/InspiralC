using System.Collections.Generic;

namespace inspiral
{
	internal partial class Modules
	{
		internal BodyModule Bodies = new BodyModule();
	}
	internal class BodyModule : GameModule
	{
		internal Dictionary<string, Bodyplan> plans = new Dictionary<string, Bodyplan>();
		internal Dictionary<string, Bodypart> parts = new Dictionary<string, Bodypart>();
		internal override void Initialize() 
		{
			Game.LogError("Done.\nLoading bodyplan definitions.");
			// TODO bodyplan/bodypart repo
			Game.LogError("Done.");
		}
		internal Bodyplan? GetPlan(string name)
		{
			name = name.ToLower();
			if(plans.ContainsKey(name))
			{
				return plans[name];
			 }
			 return null;
		}
		internal Bodypart? GetPart(string name)
		{
			name = name.ToLower();
			if(parts.ContainsKey(name))
			{
				return parts[name];
			 }
			 return null;
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
		internal List<string> equipmentSlots = new List<string>();
		internal List<System.Tuple<string, string>> attackStrings = new List<System.Tuple<string, string>>();

		internal Bodypart(string _name, string _parent, List<string> _slots)
		{
			name = _name;
			parent = _parent;
			equipmentSlots = _slots;
		}
	}
}