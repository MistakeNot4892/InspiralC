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
	internal static partial class Modules
	{
		internal static GenderModule Gender { get { return (GenderModule)GetModule<GenderModule>(); } }
	}
	internal class GenderModule : GameModule
	{
		internal GenderObject DefaultGender = new GenderObject();
		internal override void Initialize() 
		{
			Game.LogError("Loading gender definitions.");
			if(GenderObject.Tokens.Contains("it"))
			{
				GenderObject.Tokens.Remove("it");
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
			if(GenderObject.AllGenders.ContainsKey(term))
			{
				return GenderObject.AllGenders[term];
			}
			return DefaultGender;
		}
	}
}
