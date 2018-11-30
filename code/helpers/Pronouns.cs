using System.Collections.Generic;
using BCrypt;

namespace inspiral
{
	internal static class Pronouns
	{
		internal enum Gender {Male = 0, Female = 1, Neuter = 2, Plural = 3};
		internal static string His(this Gender g) {
			switch(g)
			{
				case Gender.Male:
					return "his";
				case Gender.Female:
					return "her";
				case Gender.Neuter:
					return "its";
				default:
					return "their";
			}
		}
		internal static string Him(this Gender g) {
			switch(g)
			{
				case Gender.Male:
					return "him";
				case Gender.Female:
					return "her";
				case Gender.Neuter:
					return "it";
				default:
					return "them";
			}
		}
		internal static string He(this Gender g) {
			switch(g)
			{
				case Gender.Male:
					return "he";
				case Gender.Female:
					return "she";
				case Gender.Neuter:
					return "it";
				default:
					return "they";
			}
		}
		internal static string Is(this Gender g) {
			switch(g)
			{
				case Gender.Plural:
					return "are";
				default:
					return "is";
			}
		}
	}
}
