namespace inspiral
{
	internal static partial class Gender
	{
		internal const string Androgyne = "androgyne";
	} 
	internal class AndrogyneGender : GenderObject
	{
		internal override string Term { get; set; } = Gender.Androgyne;
		internal override string His  { get; set; } = "their";
		internal override string Him  { get; set; } = "them";
		internal override string He   { get; set; } = "they";
		internal override string Is   { get; set; } = "are";
		internal override string Self { get; set; } = "selves";
	}
}