using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal static partial class Field
	{
		internal const string CanGrasp = "cangrasp";
		internal const string CanStand = "canstand";
		internal const string NaturalWeapon = "isnaturalweapon";
		internal const string EquipmentSlots = "equipmentslots";
	}
	internal class BodypartBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(BodypartComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Field.CanGrasp,       (typeof(int),    "0",  false, false) },
				{ Field.CanStand,       (typeof(int),    "0",  false, false) },
				{ Field.NaturalWeapon,  (typeof(int),    "0",  false, false) },
				{ Field.EquipmentSlots, (typeof(string), "''", false, false) }
			};
			base.Initialize();
		}
	}
	internal class BodypartComponent : GameComponent
	{
		private bool canGrasp = false;
		private bool canStand = false;
		private bool isNaturalWeapon = false;
		private List<string> equipmentSlots = new List<string>();
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = new Dictionary<string, object>();
			saveData.Add(Field.CanGrasp,       (canGrasp ? 1 : 0));
			saveData.Add(Field.CanStand,       (canStand ? 1 : 0));
			saveData.Add(Field.NaturalWeapon,  (isNaturalWeapon ? 1 : 0));
			saveData.Add(Field.EquipmentSlots, JsonConvert.SerializeObject(equipmentSlots));
			return saveData;
		}
		internal override void CopyFromRecord(DatabaseRecord record) 
		{
			base.CopyFromRecord(record);
			canGrasp =        ((long)record.fields[Field.CanGrasp] == 1);
			canStand =        ((long)record.fields[Field.CanStand] == 1);
			isNaturalWeapon = ((long)record.fields[Field.NaturalWeapon] == 1);
			equipmentSlots =  JsonConvert.DeserializeObject<List<string>>((string)record.fields[Field.EquipmentSlots]);
		}
	}
}
