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
		public Dictionary<string, GenderObject> AllGenders = new Dictionary<string, GenderObject>();
		public List<string> Tokens = new List<string>();

		internal GenderObject DefaultGender;
		internal GenderModule()
		{
			DefaultGender = new GenderObject(this);
		} 
		internal override void Initialize() 
		{
			Program.Game.LogError("Loading gender definitions.");
			if(Tokens.Contains("it"))
			{
				Tokens.Remove("it");
			}
			Program.Game.LogError($"Done.");
		}
		internal GenderObject GetByTerm(string? term)
		{
			if(term == null)
			{
				return DefaultGender;
			}
			term = term.ToLower();
			if(AllGenders.ContainsKey(term))
			{
				return AllGenders[term];
			}
			return DefaultGender;
		}
	}
}
