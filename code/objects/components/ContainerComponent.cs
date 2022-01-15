using System.Collections.Generic;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Containers => GetComponents<ContainerComponent>();
	}

	internal static partial class Field
	{
		internal static DatabaseField IsOpen = new DatabaseField(
			"isopen", true,
		     typeof(bool), true, true);
		internal static DatabaseField HasLid = new DatabaseField(
			"haslid", false,
			typeof(bool), true, true);
		internal static DatabaseField MaxCapacity = new DatabaseField(
			"maxcapacity", 10, 
			typeof(long), true, true);

	}
	internal class ContainerBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(ContainerComponent);
			schemaFields = new List<DatabaseField>() { Field.IsOpen, Field.HasLid, Field.MaxCapacity };
			base.Initialize();
		}
	}
	class ContainerComponent : GameComponent {}
}
