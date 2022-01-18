using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal static partial class Field
	{
		internal static DatabaseField CanGrasp = new DatabaseField(
			"cangrasp", false,
			typeof(bool), false, false);
		internal static DatabaseField CanStand = new DatabaseField(
			"canstand", false,
			typeof(bool), false, false);
		internal static DatabaseField NaturalWeapon = new DatabaseField(
			"isnaturalweapon", false, 
			typeof(bool), false, false);
		internal static DatabaseField EquipmentSlots = new DatabaseField(
			"equipmentslots", "", 
			typeof(string), false, false);
	}
	internal class BodypartBuilder : GameComponentBuilder
	{
		internal override void Initialize()
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
			base.Initialize();
		}
	}
	internal class BodypartComponent : GameComponent {}
}
