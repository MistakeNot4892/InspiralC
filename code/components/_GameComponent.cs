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
		internal override string TableSchema  { get; set; } = "CREATE TABLE IF NOT EXISTS foo () VALUES ();";
		internal override string LoadSchema   { get; set; } = "SELECT * FROM foo WHERE id = @p0;"
		internal override string UpdateSchema { get; set; } = "UPDATE foo;";
		internal override string InsertSchema { get; set; } = "INSERT INTO foo VALUES () WHERE (id == @p0);";
		internal FooBuilder()
		{
			ComponentType = typeof(FooComponent);
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
		internal GameComponentBuilder()              { Initialize(); }
		internal virtual void Initialize()           {}
		internal virtual System.Type ComponentType   { get; set; } = null;
		internal virtual string TableSchema          { get; set; } = null;
		internal virtual string UpdateSchema         { get; set; } = null;
		internal virtual string InsertSchema         { get; set; } = null;
		internal virtual string LoadSchema           { get; set; } = null;
		internal virtual List<string> editableFields { get; set; } = null;
		internal virtual List<string> viewableFields { get; set; } = null;
	}

	internal class GameComponent
	{
		internal GameComponent() { Initialize(); }
		internal virtual void Initialize() {}
		internal GameEntity parent;
		internal bool isPersistent = true;

		internal virtual void FinalizeObjectLoad() {}

		internal virtual void Added(GameEntity addedTo)
		{
			parent = addedTo;
			if(isPersistent)
			{
				Game.Objects.QueueForUpdate(parent);
			}
		}
		internal virtual void Removed(GameEntity takenFrom)
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

		internal string GetStringSummary() 
		{
			System.Type myType = this.GetType();
			if(Modules.Components.builders.ContainsKey(myType) && 
				Modules.Components.builders[myType].viewableFields != null && 
				Modules.Components.builders[myType].viewableFields.Count > 0)
			{
				string result = "";
				foreach(string field in Modules.Components.builders[myType].viewableFields)
				{
					if(Modules.Components.builders[myType].editableFields != null && 
						Modules.Components.builders[myType].editableFields.Contains(field))
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
			return null;
		}

		internal string SetValueOfEditableField(string field, string value) 
		{
			System.Type myType = this.GetType();
			if(Modules.Components.builders.ContainsKey(myType) && 
				Modules.Components.builders[myType].editableFields != null && 
				Modules.Components.builders[myType].editableFields.Count > 0)
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
