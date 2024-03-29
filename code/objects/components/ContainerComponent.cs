using System.Collections.Generic;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Containers => Repositories.Components.GetComponents("container");
	}

	internal static partial class Field
	{
		internal static DatabaseField IsOpen = new DatabaseField(
			"isopen", true,
		     typeof(bool), true, true, false);
		internal static DatabaseField HasLid = new DatabaseField(
			"haslid", false,
			typeof(bool), true, true, false);
		internal static DatabaseField MaxCapacity = new DatabaseField(
			"maxcapacity", 10, 
			typeof(int), true, true, false);

	}
	internal class ContainerBuilder : GameComponentBuilder
	{
		public ContainerBuilder()
		{
			tableName = "containers";
			ComponentId = "container";
			schemaFields = new List<DatabaseField>() 
			{
				Field.Id,
				Field.Parent,
				Field.ComponentId,
				Field.IsOpen,
				Field.HasLid,
				Field.MaxCapacity
			};
		}
		internal override GameComponent MakeComponent()
		{
			return new ContainerComponent();
		}

	}
	class ContainerComponent : GameComponent {}
}
