using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class Text
	{
		internal const string FieldCanGrasp = "cangrasp";
		internal const string FieldCanStand = "canstand";
		internal const string FieldNaturalWeapon = "isnaturalweapon";
		internal const string FieldEquipmentSlots = "equipmentslots";
	}
	internal class BodypartBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(BodypartComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Text.FieldCanGrasp,       (typeof(int),    "0",  false, false) },
				{ Text.FieldCanStand,       (typeof(int),    "0",  false, false) },
				{ Text.FieldNaturalWeapon,  (typeof(int),    "0",  false, false) },
				{ Text.FieldEquipmentSlots, (typeof(string), "''", false, false) }
			};
			base.Initialize();
		}
	}
	internal class BodypartComponent : GameComponent
	{
		internal bool canGrasp = false;
		internal bool canStand = false;
		internal bool isNaturalWeapon = false;
		internal List<string> equipmentSlots = new List<string>();
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = new Dictionary<string, object>();
			saveData.Add(Text.FieldCanGrasp,       (canGrasp ? 1 : 0));
			saveData.Add(Text.FieldCanStand,       (canStand ? 1 : 0));
			saveData.Add(Text.FieldNaturalWeapon,  (isNaturalWeapon ? 1 : 0));
			saveData.Add(Text.FieldEquipmentSlots, JsonConvert.SerializeObject(equipmentSlots));
			return saveData;
		}
		internal override void CopyFromRecord(DatabaseRecord record) 
		{
			base.CopyFromRecord(record);
			canGrasp =        ((long)record.fields[Text.FieldCanGrasp] == 1);
			canStand =        ((long)record.fields[Text.FieldCanStand] == 1);
			isNaturalWeapon = ((long)record.fields[Text.FieldNaturalWeapon] == 1);
			equipmentSlots =  JsonConvert.DeserializeObject<List<string>>((string)record.fields[Text.FieldEquipmentSlots]);
		}
	}
}
