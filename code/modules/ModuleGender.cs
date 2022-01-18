using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace inspiral
{

	internal static partial class Text
	{
		internal const string GenderInanimate = "inanimate";
	}
	internal partial class Modules
	{
		internal GenderModule Gender = new GenderModule();
	}
	internal class GenderModule : GameModule
	{
		public Dictionary<string, GenderObject> genders = new Dictionary<string, GenderObject>();
		public GenderObject DefaultGender = new GenderObject();
		public List<string> Tokens = new List<string>();

		internal override void Initialize() 
		{
			Game.LogError("Creating default gender definition.");
			new GenderObject();
			Game.LogError("Loading additional gender definitions.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data/definitions/genders", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Game.LogError($"- Loading gender definition {f.File}.");
				try
				{
					var textToConvert = File.ReadAllText(f.File);
					if(textToConvert != null)
					{
						Dictionary<string, string>? genderStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(textToConvert);
						if(genderStrings != null)
						{
							new GenderObject(
								genderStrings["Term"], 
								genderStrings["They"], 
								genderStrings["Them"], 
								genderStrings["Their"], 
								genderStrings["Theirs"], 
								genderStrings["Is"], 
								genderStrings["Self"]
							);
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
		internal GenderObject GetByTerm(string? term)
		{
			if(term == null)
			{
				return DefaultGender;
			}
			term = term.ToLower();
			if(genders.ContainsKey(term))
			{
				return genders[term];
			}
			return DefaultGender;
		}
	}
}
