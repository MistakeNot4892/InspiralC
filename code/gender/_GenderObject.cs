using System.Collections.Generic;

namespace inspiral
{
	internal class GenderObject
	{
        internal string They   = "it";
        internal string Them   = "it";
        internal string Their  = "its";
        internal string Theirs = "its";
		internal string Is     = "is";
        internal string Self   = "self";
        internal string Term   = Text.GenderInanimate;
		internal GenderObject(GenderModule genderMod)
		{
			Initialize(genderMod);
		}
		internal GenderObject(GenderModule genderMod, string _term, string _they, string _them, string _their, string _theirs, string _is, string _self)
		{
			Term = _term;
			They = _they;
			Them = _them;
			Their = _their;
			Theirs = _theirs;
			Is = _is;
			Self = _self;
			Initialize(genderMod);
		}
		internal void Initialize(GenderModule genMod)
		{
			genMod.AllGenders.Add(Term, this);
			foreach(string token in new System.Collections.Generic.List<string>() {They, Them, Their, Theirs, Is, Self})
			{
				if(!genMod.Tokens.Contains(token))
				{
					genMod.Tokens.Add(token);
				}
			}
		}
	}
}