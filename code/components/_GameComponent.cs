using System;
using System.Data.SQLite;

/* Template for copypasting to new files:
namespace inspiral
{
	internal static partial class Components
	{
		internal const string Foo = "foo";
	}
	internal class FooBuilder : GameComponentBuilder
	{
		internal override string Name         { get; set; } = Components.Foo;
		internal override string TableSchema  { get; set; } = "CREATE TABLE IF NOT EXISTS foo () VALUES ();";
		internal override string LoadSchema   { get; set; } = "SELECT * FROM foo WHERE id = @p0;"
		internal override string UpdateSchema { get; set; } = "UPDATE foo;";
		internal override string InsertSchema { get; set; } = "INSERT INTO foo VALUES () WHERE (id == @p0);";
		internal override GameComponent Build()
		{
			return new FooComponent();
		}
	}
	internal class FooComponent : GameComponent
	{
		internal void FooMethod() {};
	}
}
*/

namespace inspiral
{

	internal class GameComponentBuilder
	{
		internal virtual string Name         { get; set; } = null;
		internal virtual string TableSchema  { get; set; } = null;
		internal virtual string UpdateSchema { get; set; } = null;
		internal virtual string InsertSchema { get; set; } = null;
		internal virtual string LoadSchema   { get; set; } = null;
		internal virtual GameComponent Build() { return null; }
	}

	internal class GameComponent
	{
		internal string name;
		internal GameObject parent;
		internal virtual void Added(GameObject addedTo)
		{
			parent = addedTo;
			Game.Objects.QueueForUpdate(parent);
		}
		internal virtual void Removed(GameObject takenFrom)
		{
			if(takenFrom == parent)
			{
				parent = null;
			}
			Game.Objects.QueueForUpdate(takenFrom);
		}
		internal virtual bool SetValue(string key, string newValue) { return false; }
		internal virtual bool SetValue(string key, long newValue) { return false; }
		internal virtual string GetString(string key) { return null; }
		internal virtual long GetLong(string key) { return 0; }
		internal virtual void InstantiateFromRecord(SQLiteDataReader reader) {}
		internal virtual void AddCommandParameters(SQLiteCommand command) {}
		internal virtual string GetStringSummary() { return "No values."; }
	}
}
