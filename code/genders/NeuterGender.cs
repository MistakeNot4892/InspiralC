namespace inspiral
{
	internal static partial class Gender
	{
		internal const string Neuter = "neuter";
	} 

	internal class NeuterGender : GenderObject
	{
		internal override string Term { get; set; } = Gender.Neuter;
		internal override string His  { get; set; } = "eir";
		internal override string Him  { get; set; } = "em";
		internal override string He   { get; set; } = "ey";
		internal override string Is   { get; set; } = "is";
		internal override string Self { get; set; } = "self";
	}
}