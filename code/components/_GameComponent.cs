using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace inspiral
{

	internal class GameComponentBuilder
	{
		internal GameComponentBuilder()
		{
			Initialize();
		}
		internal System.Type ComponentType;
		internal List<string> editableFields = new List<string>();
		internal List<string> viewableFields = new List<string>();
		internal Dictionary<string, (System.Type, string, bool, bool)> schemaFields = null;
		internal virtual void Initialize()
		{
			if(schemaFields != null)
			{
				foreach(KeyValuePair<string, (System.Type, string, bool, bool)> schemaField in schemaFields)
				{
					if(schemaField.Value.Item3)
					{
						viewableFields.Add(schemaField.Key);
					}
					if(schemaField.Value.Item4)
					{
						editableFields.Add(schemaField.Key);
					}
				}
			}
		}
	}

	internal class GameComponent : GameEntity
	{
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
		internal virtual string GetPrompt()
		{
			return "";
		}
	}
}
