using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Wearables => GetComponents<WearableComponent>();
	}
	internal static partial class Text
	{
		internal const string FieldWearableSlots = "wearable_slots";
	}
	internal class WearableBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(WearableComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Text.FieldWearableSlots, (typeof(string), "''", true, true) }
			};
			base.Initialize();
		}
	}
	class WearableComponent : GameComponent 
	{
		internal List<string> wearableSlots = new List<string>();
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = base.GetSaveData();
			saveData.Add(Text.FieldWearableSlots, JsonConvert.SerializeObject(wearableSlots));
			return saveData;
		}
		internal override void CopyFromRecord(DatabaseRecord record)
		{
			base.CopyFromRecord(record);
			wearableSlots = JsonConvert.DeserializeObject<List<string>>(record.fields[Text.FieldWearableSlots].ToString());
		}
	}
}