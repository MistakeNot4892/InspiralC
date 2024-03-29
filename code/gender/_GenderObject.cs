using System.Collections.Generic;

namespace inspiral
{
	internal class GenderObject
	{
		internal static Dictionary<string, GenderObject> AllGenders = new Dictionary<string, GenderObject>();
		internal static List<string> Tokens = new List<string>();

        internal string They   = "it";
        internal string Them   = "it";
        internal string Their  = "its";
        internal string Theirs = "its";
		internal string Is     = "is";
        internal string Self   = "self";
        internal string Term   = Text.GenderInanimate;
		internal GenderObject()
		{
			Initialize();
		}
		internal GenderObject(string _term, string _they, string _them, string _their, string _theirs, string _is, string _self)
		{
			Term = _term;
			They = _they;
			Them = _them;
			Their = _their;
			Theirs = _theirs;
			Is = _is;
			Self = _self;
			Initialize();
		}
		internal void Initialize()
		{
			AllGenders.Add(Term, this);
			foreach(string token in new System.Collections.Generic.List<string>() {They, Them, Their, Theirs, Is, Self})
			{
				if(!Tokens.Contains(token))
				{
					Tokens.Add(token);
				}
			}
		}
	}
}