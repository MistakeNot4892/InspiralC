using System.Collections.Generic;
using System.Data.SQLite;

namespace inspiral
{
    internal static partial class Database
    {
        internal static object ConvertDataForSave(KeyValuePair<DatabaseField, object> recordField)
        {
            if(recordField.Key.fieldType == typeof(bool))
            {
                return (bool)recordField.Value ? 1 : 0;
            }
            return recordField.Value;

        }
        // Would be nice if we could make a generic using the fieldType of DatabaseField instead of needing the big else-if below
        internal static void CastAndAdd<T>(Dictionary<DatabaseField, object> fields, DatabaseField field, object val)
        {
            T castVal = (T)val;
            fields.Add(field, castVal);
        }
        internal static Dictionary<DatabaseField, object> GetDataFromRecord(SQLiteDataReader record)
        {
            Dictionary<DatabaseField, object> fields = new Dictionary<DatabaseField, object>();
            for(int i=0;i<record.FieldCount;i++)
            {
                DatabaseField field = Field.GetFieldFromName(record.GetName(i));
                if(!field.IsField(Field.Dummy))
                {
                    // No value to load somehow.
                    object fieldVal = record[field.fieldName];
                    if(fieldVal == null)
                    {
                        continue;
                    }
                    else if(field.fieldType == typeof(bool))
                    {
                        CastAndAdd<bool>(fields, field, ((int)fieldVal >= 1));
                    }
                    else if(field.fieldType == typeof(string))
                    {
                        CastAndAdd<string>(fields, field, fieldVal);
                    }
                    else if(field.fieldType == typeof(double))
                    {
                        CastAndAdd<double>(fields, field, fieldVal);
                    }
                    else if(field.fieldType == typeof(ulong))
                    {
                        CastAndAdd<ulong>(fields, field, fieldVal);
                    }
                    else
                    {
                        CastAndAdd<int>(fields, field, fieldVal);
                    }
                }
            }
            return fields;
        }
    }
    internal class SQLiteDatabaseHandler : DatabaseHandler
    {
        private static Dictionary<System.Type, string> fieldTypes = new Dictionary<System.Type, string>()
        {
            { typeof(ulong),  "UNSIGNED BIG INT" },
            { typeof(string), "TEXT"    },
            { typeof(int),    "INTEGER" },
            { typeof(bool),   "INTEGER" },
            { typeof(double), "DOUBLE"  }
        };
        private string GetFieldTypeString(System.Type fieldType)
        {
            if(fieldTypes.ContainsKey(fieldType))
            {
                return fieldTypes[fieldType];
            }
            return "BLOB";
        }

        SQLiteConnection connection;
        internal SQLiteDatabaseHandler(string dbPath, string dbVersion) : base(dbPath, dbVersion)
        {
            connection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
        }
        internal override void CreateRecord(string tableName, IGameEntity entity) 
        {
            using(SQLiteCommand command = new SQLiteCommand($"INSERT INTO {tableName} ( id ) VALUES ( @id );", connection))
            {
                command.Parameters.AddWithValue("@id", entity.GetValue<ulong>(Field.Id));
                command.ExecuteNonQuery();
                UpdateRecord(tableName, entity);
            }
        }
        internal override void UpdateRecord(string tableName, IGameEntity entity) 
        {
            Dictionary<DatabaseField, object> entityFields = entity.GetSaveData();
            List<string> updateStrings = new List<string>();
            foreach(DatabaseField recordKey in entityFields.Keys)
            {
                updateStrings.Add($"{recordKey.fieldName} = @{recordKey.fieldName}");
            }
            using(SQLiteCommand command = new SQLiteCommand($"UPDATE {tableName} SET {string.Join(", ", updateStrings)} WHERE id = @id;", connection))
            {
                foreach(KeyValuePair<DatabaseField, object> recordValue in entityFields)
                {
                    command.Parameters.AddWithValue($"@{recordValue.Key.fieldName}", Database.ConvertDataForSave(recordValue));
                }
                command.ExecuteNonQuery();
            }
        }
        internal override void InitializeTable(string tableName, List<DatabaseField> tableFields)
        {
            List<string> tableFieldStrings = new List<string>();
            foreach(DatabaseField field in tableFields)
            {
                string tableFieldString = $"{field.fieldName} {GetFieldTypeString(field.fieldType)}";
                if(field.fieldDefault != null)
                {
                    if(field == Field.Id)
                    {
                        tableFieldString = "id UNSIGNED BIG INT PRIMARY KEY UNIQUE";
                    }
                    else if(field.fieldType == typeof(string))
                    {
                        tableFieldString = $"{tableFieldString} DEFAULT '{field.fieldDefault}'";
                    }
                    else
                    {
                        tableFieldString = $"{tableFieldString} DEFAULT {field.fieldDefault}";
                    }
                }
                tableFieldStrings.Add(tableFieldString);
            }
            string tableSchema = $"CREATE TABLE IF NOT EXISTS {tableName} ( {string.Join(", ", tableFieldStrings)} );";
            using(SQLiteCommand command = new SQLiteCommand(tableSchema, connection))
            {
                command.ExecuteNonQuery();
            }            
        }
        internal override List<Dictionary<DatabaseField, object>> GetAllRecords(string tableName)
        { 
            List<Dictionary<DatabaseField, object>> records = base.GetAllRecords(tableName);
            using(SQLiteCommand command = new SQLiteCommand($"SELECT * FROM {tableName};", connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    records.Add(Database.GetDataFromRecord(reader));
                }
            }
            return records;
        }
        internal override void BatchUpdateRecords(string tableName, List<IGameEntity> updateQueue) 
        {
			var saveTransaction = connection.BeginTransaction();
			while(updateQueue.Count > 0)
			{
                IGameEntity objInstance = updateQueue[0];
                UpdateRecord(tableName, objInstance);
                updateQueue.Remove(objInstance);
			}
			saveTransaction.Commit();            
        }

        internal override void Open()
        {
            connection.Open();
        }
        internal override void Close()
        {
            connection.Close();
        }
    }
}   
