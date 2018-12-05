namespace inspiral
{
	internal static partial class Gender
	{
		internal const string Male = "male";
	} 

	internal class MaleGender : GenderObject
	{
		internal override string Term { get; set; } = Gender.Male;
		internal override string His  { get; set; } = "his";
		internal override string Him  { get; set; } = "him";
		internal override string He   { get; set; } = "he";
		internal override string Is   { get; set; } = "is";
		internal override string Self { get; set; } = "self";
	}
}