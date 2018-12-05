using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace inspiral
{

	internal class GenderObject
	{
		internal virtual string Term { get; set; } = null;
		internal virtual string His  { get; set; } = null;
		internal virtual string Him  { get; set; } = null;
		internal virtual string He   { get; set; } = null;
		internal virtual string Is   { get; set; } = null;
		internal virtual string Self { get; set; } = null;
	}

	internal static partial class Gender
	{
		private static Dictionary<string, GenderObject> genders = new Dictionary<string, GenderObject>();
		internal static GenderObject GetByTerm(string term)
		{
			term = term.ToLower();
			if(genders.ContainsKey(term))
			{
				return genders[term];
			}
			return null;
		}
		static Gender()
		{
			Debug.WriteLine($"Loading genders.");
			foreach(var t in (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GenderObject))
				select assemblyType))
			{
				Debug.WriteLine($"Loading gender {t}.");
				GenderObject gender = (GenderObject)Activator.CreateInstance(t);
				genders.Add(gender.Term, gender);
			}
			Debug.WriteLine($"Done.");
		}
	}
}
