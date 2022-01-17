using System.Collections.Generic;

namespace inspiral
{

	internal class GameComponentBuilder
	{
		internal GameComponentBuilder()
		{
			Initialize();
		}
		internal System.Type ComponentType;
		internal List<DatabaseField> editableFields = new List<DatabaseField>();
		internal List<DatabaseField> viewableFields = new List<DatabaseField>();
		internal List<DatabaseField> schemaFields = null;
		internal virtual void Initialize()
		{
			if(schemaFields != null)
			{
				foreach(DatabaseField schemaField in schemaFields)
				{
					if(schemaField.fieldIsViewable)
					{
						viewableFields.Add(schemaField);
					}
					if(schemaField.fieldIsEditable)
					{
						editableFields.Add(schemaField);
					}
				}
			}
		}
	}

	internal class GameComponent : GameObject
	{
		internal GameObject parent;
		internal bool isPersistent = true;
		internal GameComponent() { InitializeComponent(); }
		internal virtual void InitializeComponent() {}
		internal virtual void FinalizeObjectLoad() {}

		internal virtual void Added(GameObject addedTo)
		{
			parent = addedTo;
			if(isPersistent)
			{
				Repos.Objects.QueueForUpdate(parent);
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
				Repos.Objects.QueueForUpdate(takenFrom);
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
				foreach(DatabaseField field in Modules.Components.builders[myType].viewableFields)
				{
					if(Modules.Components.builders[myType].editableFields != null && 
						Modules.Components.builders[myType].editableFields.Contains(field))
					{
						result = $"{result}\n{field}: {GetValue<string>(field)}";
					}
					else
					{
						result = $"{result}\n{field} (read-only): {GetValue<string>(field)}";
					}
				}
				return result;
			}
			return null;
		}

		internal string SetValueOfEditableField(DatabaseField field, string value) 
		{
			System.Type myType = this.GetType();
			if(Modules.Components.builders.ContainsKey(myType) && 
				Modules.Components.builders[myType].editableFields != null && 
				Modules.Components.builders[myType].editableFields.Count > 0)
			{
				SetValue(field, value);
				Repos.Objects.QueueForUpdate(parent);
				return null;	
			}
			return "Invalid field.";
		}
		internal virtual string GetPrompt()
		{
			return "";
		}
	}
}
