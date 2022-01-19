using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Field
	{
		internal static DatabaseField CanGrasp = new DatabaseField(
			"cangrasp", false,
			typeof(bool), false, false, false);
		internal static DatabaseField CanStand = new DatabaseField(
			"canstand", false,
			typeof(bool), false, false, false);
		internal static DatabaseField NaturalWeapon = new DatabaseField(
			"isnaturalweapon", false, 
			typeof(bool), false, false, false);
		internal static DatabaseField EquipmentSlots = new DatabaseField(
			"equipmentslots", "", 
			typeof(string), false, false, true);
	}
	internal class BodypartBuilder : GameComponentBuilder
	{
		public BodypartBuilder()
		{
			ComponentType = typeof(BodypartComponent);
			schemaFields = new List<DatabaseField>()
			{
				Field.Parent,
				Field.CanGrasp,
				Field.CanStand,
				Field.NaturalWeapon,
				Field.EquipmentSlots
			};
		}
	}
	internal class BodypartComponent : GameComponent 
	{
		internal List<string> equipmentSlots = new List<string>();
	}
}
