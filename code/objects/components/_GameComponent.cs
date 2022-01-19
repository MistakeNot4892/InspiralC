using System.Collections.Generic;

namespace inspiral
{

	internal static partial class Field
	{
		internal static DatabaseField Parent = new DatabaseField(
			"parent", 0,
			typeof(int), true, false);
	}
	internal class GameComponentBuilder
	{
		internal GameComponentBuilder()
		{
			Initialize();
		}
		internal System.Type? ComponentType;
		internal List<DatabaseField> editableFields = new List<DatabaseField>();
		internal List<DatabaseField> viewableFields = new List<DatabaseField>();
		internal List<DatabaseField> schemaFields = new List<DatabaseField>();
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
				Program.Game.Repos.Objects.QueueForUpdate(addedTo);
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
				Program.Game.Repos.Objects.QueueForUpdate(takenFrom);
			}
		}
		internal string GetStringSummary() 
		{
			System.Type myType = this.GetType();
			if(Program.Game.Mods.Components.builders.ContainsKey(myType) && 
				Program.Game.Mods.Components.builders[myType].viewableFields != null && 
				Program.Game.Mods.Components.builders[myType].viewableFields.Count > 0)
			{
				string result = "";
				foreach(DatabaseField field in Program.Game.Mods.Components.builders[myType].viewableFields)
				{
					if(Program.Game.Mods.Components.builders[myType].editableFields != null && 
						Program.Game.Mods.Components.builders[myType].editableFields.Contains(field))
					{
						result = $"{result}\n{field.fieldName}: {GetValue<string>(field)}";
					}
					else
					{
						result = $"{result}\n{field.fieldName} (read-only): {GetValue<string>(field)}";
					}
				}
				return result;
			}
			return "";
		}
		internal bool SetValueOfEditableField<T>(DatabaseField field, T value) 
		{
			System.Type myType = this.GetType();
			if(Program.Game.Mods.Components.builders.ContainsKey(myType) && 
				Program.Game.Mods.Components.builders[myType].editableFields != null && 
				Program.Game.Mods.Components.builders[myType].editableFields.Count > 0)
			{
				SetValue<T>(field, value);
				Program.Game.Repos.Objects.QueueForUpdate(this);
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
		}
		public Dictionary<DatabaseField, object> GetSaveData()
		{
			return Fields;
		}
		internal GameObject? GetParent()
		{
			var parent = Program.Game.Repos.Objects.GetById(GetValue<ulong>(Field.Parent));
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
