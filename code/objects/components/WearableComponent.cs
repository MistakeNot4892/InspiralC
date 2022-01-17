using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Wearables => GetComponents<WearableComponent>();
	}
	internal static partial class Field
	{
		internal static DatabaseField WearableSlots = new DatabaseField(
			"wearable_slots", "",
			typeof(List<string>), true, true);
	}
	internal class WearableBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(WearableComponent);
			schemaFields = new List<DatabaseField>() { Field.WearableSlots };
			base.Initialize();
		}
	}
	class WearableComponent : GameComponent 
	{
		internal List<string> wearableSlots = new List<string>();
	}
}