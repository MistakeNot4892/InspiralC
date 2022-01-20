using System.Collections.Generic;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Wearables => Repositories.Components.GetComponents<WearableComponent>();
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
				Field.Id,
				Field.Parent,
				Field.WearableSlots
			};
		}
		internal override GameComponent MakeComponent()
		{
			return new WearableComponent();
		}
	}
	class WearableComponent : GameComponent 
	{
		internal List<string> wearableSlots = new List<string>();
	}
}