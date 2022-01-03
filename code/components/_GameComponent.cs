using System.Data.SQLite;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

/* Template for copypasting to new files:
namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal const string Foo = "foo";
	}
	internal static partial class Text
	{
		internal const string FieldFoo = "foofield";
		internal const string FieldBar = "barfield";
	}
	internal class FooBuilder : GameComponentBuilder
	{
		internal override string Name         { get; set; } = Text.CompFoo;
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
		internal virtual List<string> editableFields { get; set; } = new List<string>() { Text.FieldFoo, Text.FieldBar };
		internal virtual List<string> viewableFields { get; set; } = new List<string>() { Text.FieldBar };
	}
}
*/

namespace inspiral
{

	internal class GameComponentBuilder
	{
		internal virtual string Name                 { get; set; } = null;
		internal virtual string TableSchema          { get; set; } = null;
		internal virtual string UpdateSchema         { get; set; } = null;
		internal virtual string InsertSchema         { get; set; } = null;
		internal virtual string LoadSchema           { get; set; } = null;
		internal virtual List<string> editableFields { get; set; } = null;
		internal virtual List<string> viewableFields { get; set; } = null;
		internal virtual GameComponent Build() { return null; }
	}

	internal class GameComponent
	{
		internal string name;
		internal GameObject parent;
		internal bool isPersistent = true;

		internal virtual void FinalizeObjectLoad() {}

		internal virtual void Added(GameObject addedTo)
		{
			parent = addedTo;
			if(isPersistent)
			{
				Game.Objects.QueueForUpdate(parent);
			}
		}
		internal virtual void Removed(GameObject takenFrom)
		{
			if(takenFrom == parent)
			{
				parent = null;
			}
			if(isPersistent)
			{
				Game.Objects.QueueForUpdate(takenFrom);
			}
		}

		internal string GetStringSummary() {
			if(Modules.Components.builders.ContainsKey(name) && 
				Modules.Components.builders[name].viewableFields != null && 
				Modules.Components.builders[name].viewableFields.Count > 0)
			{
				string result = "";
				foreach(string field in Modules.Components.builders[name].viewableFields)
				{
					if(Modules.Components.builders[name].editableFields != null && 
						Modules.Components.builders[name].editableFields.Contains(field))
					{
						result = $"{result}\n{field}: {GetString(field)}";
					}
					else
					{
						result = $"{result}\n{field} (read-only): {GetString(field)}";
					}
				}
				return result;
			}
			else
			{
				return "No viewable values.";
			}
		}

		internal string SetValueOfEditableField(string field, string value) 
		{
			if(Modules.Components.builders.ContainsKey(name) && 
				Modules.Components.builders[name].editableFields != null && 
				Modules.Components.builders[name].editableFields.Count > 0)
			{
				SetValue(field, value);
				Game.Objects.QueueForUpdate(parent);
				return null;	
			}
			return "Invalid field.";
		}

		internal virtual bool SetValue(string key, string newValue) { return false; }
		internal virtual bool SetValue(string key, long newValue) { return false; }
		internal virtual string GetString(string key) { return null; }
		internal virtual long GetLong(string key) { return 0; }
		internal virtual void InstantiateFromRecord(SQLiteDataReader reader) {}
		internal virtual void AddCommandParameters(SQLiteCommand command) {}
		internal virtual string GetPrompt()
		{
			return "";
		}
		internal virtual void ConfigureFromJson(JToken compData) {}
	}
}
