using System.Data.SQLite;
using System.Collections.Generic;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Containers => GetComponents(Text.CompContainer);
	}

	internal static partial class Text
	{
		internal const string CompContainer = "container";
		internal const string FieldIsOpen = "isopen";
		internal const string FieldHasLid = "haslid";
		internal const string FieldMaxCapacity = "maxcapacity";

	}
	internal class ContainerBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Text.CompContainer;
		internal override GameComponent Build()
		{
			return new ContainerComponent();
		}
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_container WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = $@"CREATE TABLE IF NOT EXISTS components_container (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				{Text.FieldIsOpen} INTEGER DEFAULT 1, 
				{Text.FieldHasLid} INTEGER DEFAULT 0,
				{Text.FieldMaxCapacity} INTEGER DEFAULT 10
				);";
		internal override string UpdateSchema   { get; set; } = $@"UPDATE components_container SET 
				{Text.FieldIsOpen} = @p1, 
				{Text.FieldHasLid} = @p2,
				{Text.FieldMaxCapacity} = @p3
				WHERE id = @p0;";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_container (
				id,
				{Text.FieldIsOpen},
				{Text.FieldHasLid},
				{Text.FieldMaxCapacity}
				) VALUES (
				@p0, 
				@p1,
				@p2,
				@p3
				);";
	}
	class ContainerComponent : GameComponent
	{
		internal bool isOpen = true;
		internal bool hasLid = false;
		internal long maxCapacity = 10;
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			isOpen =      ((long)reader[Text.FieldIsOpen] == 1);
			hasLid =      ((long)reader[Text.FieldHasLid] == 1);
			maxCapacity = (long)reader[Text.FieldMaxCapacity];
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", isOpen);
			command.Parameters.AddWithValue("@p2", hasLid);
			command.Parameters.AddWithValue("@p3", maxCapacity);
		}
	}
}
