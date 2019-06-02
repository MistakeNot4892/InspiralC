using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace inspiral
{

	internal static partial class Text
	{
		internal const string GenderInanimate = "inanimate";
		internal const string GenderPlural = "plural";
	}
	internal static partial class Modules
	{
		internal static GenderModule Gender = null;
	}

	internal class GenderObject
	{
		internal string Term = null;
		internal string His  = null;
		internal string Him  = null;
		internal string He   = null;
		internal string Is   = null;
		internal string Self = null;
	}
	internal class GenderModule : GameModule
	{
		public Dictionary<string, GenderObject> genders = new Dictionary<string, GenderObject>();
		public List<string> Tokens = new List<string>();

		internal override void Initialize() 
		{
			Modules.Gender = this;
			Debug.WriteLine($"- Loading gender definitions.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data/definitions/genders", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Debug.WriteLine($"- Loading gender definition {f.File}.");
				try
				{
					Dictionary<string, string> genderStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(f.File));
					GenderObject  gender = new GenderObject();
					gender.He =   genderStrings["He"];
					gender.Him =  genderStrings["Him"];
					gender.His =  genderStrings["His"];
					gender.Is =   genderStrings["Is"];
					gender.Term = genderStrings["Term"];
					gender.Self = genderStrings["Self"];
					genders.Add(gender.Term, gender);
					foreach(string token in new List<string>() {gender.He, gender.Him, gender.His, gender.Is, gender.Self})
					{
						if(!Tokens.Contains(token))
						{
							Tokens.Add(token);
						}
					}
				}
				catch(Exception e)
				{
					Debug.WriteLine($"Exception when loading gender from file {f.File} - {e.Message}");
				}
			}
			// 'it' is useless as it is ambiguous, just get rid of it.
			if(Tokens.Contains("it"))
			{
				Tokens.Remove("it");
			}
			Debug.WriteLine($"Done.");
		}
		internal GenderObject GetByTerm(string term)
		{
			term = term.ToLower();
			if(genders.ContainsKey(term))
			{
				return genders[term];
			}
			return null;
		}
	}
}
