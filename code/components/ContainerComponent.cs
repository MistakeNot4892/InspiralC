using System.Collections.Generic;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Containers => GetComponents<ContainerComponent>();
	}

	internal static partial class Field
	{
		internal const string IsOpen = "isopen";
		internal const string HasLid = "haslid";
		internal const string MaxCapacity = "maxcapacity";

	}
	internal class ContainerBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(ContainerComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Field.IsOpen,      (typeof(int), "1",  false, false) },
				{ Field.HasLid,      (typeof(int), "0",  false, false) },
				{ Field.MaxCapacity, (typeof(int), "10", false, false) }
			};
			base.Initialize();
		}
	}
	class ContainerComponent : GameComponent
	{
		private bool isOpen = true;
		private bool hasLid = false;
		private long maxCapacity = 10;
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = new Dictionary<string, object>();
			saveData.Add(Field.IsOpen, isOpen ? 1 : 0);
			saveData.Add(Field.HasLid, hasLid ? 1 : 0);
			saveData.Add(Field.MaxCapacity, maxCapacity);
			return saveData;
		}
		internal override void CopyFromRecord(DatabaseRecord record) 
		{
			base.CopyFromRecord(record);
			isOpen =      ((long)record.fields[Field.IsOpen] == 1);
			hasLid =      ((long)record.fields[Field.HasLid] == 1);
			maxCapacity = (long)record.fields[Field.MaxCapacity];
		}
	}
}
