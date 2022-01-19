using System.Collections.Generic;

namespace inspiral
{

	internal static partial class Field
	{
		internal static DatabaseField Parent = new DatabaseField(
			"parent", (ulong)0,
			typeof(ulong), true, false, false);
		internal static DatabaseField ComponentType = new DatabaseField(
			"componenttype", "unset",
			typeof(string), true, false, false);
	}
	internal class GameComponentBuilder
	{
		internal GameComponentBuilder()
		{
		}
		internal System.Type? ComponentType;
		internal List<DatabaseField> schemaFields = new List<DatabaseField>();
		internal virtual GameComponent MakeComponent()
		{
			return new GameComponent();
		}
	}

	internal class GameComponent : IGameEntity
	{

		private Dictionary<DatabaseField, object> _fields = new Dictionary<DatabaseField, object>();
		public Dictionary<DatabaseField, object> Fields
		{
			get { return _fields; }
			set { _fields = value; }
		}
		internal bool isPersistent = true;
		internal GameComponent() { InitializeComponent(); }
		internal virtual void InitializeComponent() {}
		internal virtual void FinalizeObjectLoad() {}
		internal virtual void Added(GameObject addedTo)
		{
			SetValue<ulong>(Field.Parent, addedTo.GetValue<ulong>(Field.Id));
			if(isPersistent)
			{
				Repositories.Objects.QueueForUpdate(addedTo);
			}
		}
		internal virtual void Removed(GameObject takenFrom)
		{
			if(takenFrom == GetParent())
			{
				SetValue<int>(Field.Parent, 0);
			}
			if(isPersistent)
			{
				Repositories.Objects.QueueForUpdate(takenFrom);
			}
		}
		internal string GetStringSummary() 
		{
			System.Type myType = this.GetType();
			if(Modules.Components.builders.ContainsKey(myType))
			{
				string result = "";
				foreach(DatabaseField field in Modules.Components.builders[myType].schemaFields)
				{
					if(!field.fieldIsViewable)
					{
						continue;
					}

					result = $"{result}\n{field.fieldName}";
					if(!field.fieldIsEditable)
					{
						result = $"{result} (read-only)";
					}

					if(field.fieldIsReference)
					{
						result = $"{result}: reference skipped";
						continue;
					}

					object? val = GetValue<object>(field);
					if(val == null)
					{
						result = $"{result}: null";
					}
					else
					{
						result = $"{result}: {val.ToString()}";
					}
				}
				return result;
			}
			return "";
		}
		internal bool SetValueOfEditableField<T>(DatabaseField field, T value) 
		{
			System.Type myType = this.GetType();
			if(field.fieldIsEditable)
			{
				SetValue<T>(field, value);
				Repositories.Objects.QueueForUpdate(this);
				return true;	
			}
			return false;
		}
		internal virtual string GetPrompt()
		{
			return "";
		}
		public bool SetValue<T>(DatabaseField field, T newValue)
		{
			if(Fields.ContainsKey(field) && newValue != null)
			{
				Fields[field] = newValue;
				return true;
			}
			return false;
		}
		public T? GetValue<T>(DatabaseField field)
		{
			if(Fields.ContainsKey(field))
			{
				return (T)Fields[field];
			}
			return default(T);
		}
		public void CopyFromRecord(Dictionary<DatabaseField, object> record) 
		{
			Fields = record;
			Fields[Field.ComponentType] = GetType().ToString();
		}
		public Dictionary<DatabaseField, object> GetSaveData()
		{
			Fields[Field.ComponentType] = GetType().ToString();
			return Fields;
		}
		internal GameObject? GetParent()
		{
			var parent = Repositories.Objects.GetById(GetValue<ulong>(Field.Parent));
			if(parent != null)
			{
				return (GameObject)parent;
			} 
			return null;
		}
		internal void WriteLine(string message)
		{
			GameObject? parent = GetParent();
			if(parent != null)
			{
				parent.WriteLine(message);
			}
		}
		internal void WriteLine(string message, bool sendPrompt)
		{
			GameObject? parent = GetParent();
			if(parent != null)
			{
				parent.WriteLine(message, sendPrompt);
			}
		}
		internal string GetColour(string colourType)
		{
			GameObject? parent = GetParent();
			if(parent != null)
			{
				return parent.GetColour(colourType);
			}
			return GlobalConfig.GetColour(colourType);
		}
	}
}
