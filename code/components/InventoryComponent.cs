using System.Data.SQLite;
using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Components
	{
		internal const string Inventory = "inventory";
		internal static List<GameComponent> Inventories => GetComponents(Inventory);
	}

	internal class InventoryBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Components.Inventory;
		internal override GameComponent Build()
		{
			return new InventoryComponent();
		}
	}

	class InventoryComponent : GameComponent 
	{
	}
}
