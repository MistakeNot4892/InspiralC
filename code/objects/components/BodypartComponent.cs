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
			tableName = "bodyparts";
			ComponentId = "bodypart";
			schemaFields = new List<DatabaseField>()
			{
				Field.Id,
				Field.Parent,
				Field.ComponentId,
				Field.CanGrasp,
				Field.CanStand,
				Field.NaturalWeapon,
				Field.EquipmentSlots
			};
		}
		internal override GameComponent MakeComponent()
		{
			return new BodypartComponent();
		}
	}
	internal class BodypartComponent : GameComponent 
	{
		internal List<string> equipmentSlots = new List<string>();
	}
}
