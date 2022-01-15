using System.Collections.Generic;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Containers => GetComponents<ContainerComponent>();
	}

	internal static partial class Text
	{
		internal const string FieldIsOpen = "isopen";
		internal const string FieldHasLid = "haslid";
		internal const string FieldMaxCapacity = "maxcapacity";

	}
	internal class ContainerBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(ContainerComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Text.FieldIsOpen,      (typeof(int), "1",  false, false) },
				{ Text.FieldHasLid,      (typeof(int), "0",  false, false) },
				{ Text.FieldMaxCapacity, (typeof(int), "10", false, false) }
			};
			base.Initialize();
		}
	}
	class ContainerComponent : GameComponent
	{
		internal bool isOpen = true;
		internal bool hasLid = false;
		internal long maxCapacity = 10;
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = new Dictionary<string, object>();
			saveData.Add(Text.FieldIsOpen, isOpen ? 1 : 0);
			saveData.Add(Text.FieldHasLid, hasLid ? 1 : 0);
			saveData.Add(Text.FieldMaxCapacity, maxCapacity);
			return saveData;
		}
		internal override void CopyFromRecord(DatabaseRecord record) 
		{
			base.CopyFromRecord(record);
			isOpen =      ((long)record.fields[Text.FieldIsOpen] == 1);
			hasLid =      ((long)record.fields[Text.FieldHasLid] == 1);
			maxCapacity = (long)record.fields[Text.FieldMaxCapacity];
		}
	}
}
