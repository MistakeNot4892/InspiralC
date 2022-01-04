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
        internal string They   = null;
        internal string Them   = null;
        internal string Their  = null;
        internal string Theirs = null;
		internal string Is     = null;
        internal string Self   = null;
        internal string Term   = null;
	}
	internal class GenderModule : GameModule
	{
		public Dictionary<string, GenderObject> genders = new Dictionary<string, GenderObject>();
		public List<string> Tokens = new List<string>();

		internal override void Initialize() 
		{
			Modules.Gender = this;
			Game.LogError($"- Loading gender definitions.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data/definitions/genders", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Game.LogError($"- Loading gender definition {f.File}.");
				try
				{
					Dictionary<string, string> genderStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(f.File));
                    var gender = new GenderObject
                    {
                        They =   genderStrings["They"],
                        Them =   genderStrings["Them"],
                        Their =  genderStrings["Their"],
                        Theirs = genderStrings["Theirs"],
                        Is =     genderStrings["Is"],
                        Self =   genderStrings["Self"],
                        Term =   genderStrings["Term"]
                    };
                    genders.Add(gender.Term, gender);
					foreach(string token in new List<string>() {gender.They, gender.Them, gender.Their, gender.Theirs, gender.Is, gender.Self})
					{
						if(!Tokens.Contains(token))
						{
							Tokens.Add(token);
						}
					}
				}
				catch(System.Exception e)
				{
					Game.LogError($"Exception when loading gender from file {f.File} - {e.Message}");
				}
			}
			// 'it' is useless as it is ambiguous, just get rid of it.
			if(Tokens.Contains("it"))
			{
				Tokens.Remove("it");
			}
			Game.LogError($"Done.");
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
