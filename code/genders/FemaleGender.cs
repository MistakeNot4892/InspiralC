namespace inspiral
{
	internal static partial class Gender
	{
		internal const string Female = "female";
	} 
	internal class FemaleGender : GenderObject
	{
		internal override string Term { get; set; } = Gender.Female;
		internal override string His  { get; set; } = "her";
		internal override string Him  { get; set; } = "she";
		internal override string He   { get; set; } = "she";
		internal override string Is   { get; set; } = "is";
		internal override string Self { get; set; } = "self";
	}
}