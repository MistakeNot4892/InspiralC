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
			"wearableslots", "[]",
			typeof(string), true, true, false);
	}
	internal class WearableBuilder : GameComponentBuilder
	{
		public WearableBuilder()
		{
			ComponentType = typeof(WearableComponent);
			schemaFields = new List<DatabaseField>()
			{
				Field.Parent,
				Field.WearableSlots
			};
		}
	}
	class WearableComponent : GameComponent 
	{
		internal List<string> wearableSlots = new List<string>();
	}
}