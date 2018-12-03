using System.Collections.Generic;
using BCrypt;

namespace inspiral
{
	internal static class Gender
	{
		internal const int Inanimate = 0;
		internal const int Male =      1;
		internal const int Female =    2;
		internal const int Neuter =    3;
		internal const int Androgyne =    4;
		internal static string His(int g) {
			switch(g)
			{
				case Male:
					return "his";
				case Female:
					return "her";
				case Inanimate:
					return "its";
				case Neuter:
					return "eir";
				default:
					return "their";
			}
		}
		internal static string Him(int g) {
			switch(g)
			{
				case Male:
					return "him";
				case Female:
					return "her";
				case Inanimate:
					return "it";
				case Neuter:
					return "em";
				default:
					return "them";
			}
		}
		internal static string He(int g) {
			switch(g)
			{
				case Male:
					return "he";
				case Female:
					return "she";
				case Inanimate:
					return "it";
				case Neuter:
					return "ey";
				default:
					return "they";
			}
		}
		internal static string Is(int g) {
			switch(g)
			{
				case Androgyne:
					return "are";
				default:
					return "is";
			}
		}
		internal static string Self(int g) {
			switch(g)
			{
				case Androgyne:
					return "selves";
				default:
					return "self";
			}
		}
	}
}