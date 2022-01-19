using System.Collections.Generic;

namespace inspiral
{
    internal static partial class Field
	{
		internal static DatabaseField Dummy = new DatabaseField(
            "dummyfield", "", typeof(string), 
            false, false, false);
		internal static Dictionary<string, DatabaseField> nameToField = new Dictionary<string, DatabaseField>();
		internal static DatabaseField GetFieldFromName(string fieldName)
		{
			if(nameToField.ContainsKey(fieldName))
			{
				return nameToField[fieldName];
			}
			return Field.Dummy;
		}
	}

    internal class DatabaseField
    {
        internal string fieldName;
        internal System.Type fieldType;
        internal object fieldDefault;
        internal bool fieldIsViewable = true;
        internal bool fieldIsReference = false;
        internal bool fieldIsEditable = false;
        // Defining this instead of overriding Equals() because hashing is scary
        internal bool IsField(DatabaseField otherField) 
        {
            return this == otherField;
        }
        internal DatabaseField(string _key, object _default, System.Type _type, bool _view, bool _edit, bool _ref)
        {
            fieldName =        _key;
            fieldType =        _type;
            fieldIsEditable =  _edit;
            fieldIsViewable =  _view;
            fieldDefault =     _default;
            fieldIsReference = _ref;
            if(Field.nameToField == null)
            {
                Field.nameToField = new Dictionary<string, DatabaseField>();
            }
            Field.nameToField.Add(fieldName, this);
        }
    }
    internal class DatabaseHandler
    {
        internal DatabaseHandler(string dbPath, string dbVersion) {}
        internal virtual List<Dictionary<DatabaseField, object>> GetAllRecords(string tableName) 
        {
            return new List<Dictionary<DatabaseField, object>>();
        }
        internal virtual void Open() {}
        internal virtual void Close() {}
        internal virtual void InitializeTable(string tableName, List<DatabaseField> tableFields) {}
        internal virtual void UpdateRecord(string tableName, IGameEntity entity) {}
        internal virtual void CreateRecord(string tableName, IGameEntity entity) {}
        internal virtual void BatchUpdateRecords(string tableName, List<IGameEntity> updateQueue) {}   
    }
}