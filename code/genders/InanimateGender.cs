namespace inspiral
{
	internal static partial class Gender
	{
		internal const string Inanimate = "inanimate";
	} 
	internal class InanimateGender : GenderObject
	{
		internal override string Term { get; set; } = Gender.Inanimate;
		internal override string His  { get; set; } = "its";
		internal override string Him  { get; set; } = "it";
		internal override string He   { get; set; } = "it";
		internal override string Is   { get; set; } = "is";
		internal override string Self { get; set; } = "self";
	}
}