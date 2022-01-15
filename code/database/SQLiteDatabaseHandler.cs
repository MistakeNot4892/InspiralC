using System.Collections.Generic;
using System.Data.SQLite;

namespace inspiral
{
    internal partial class DatabaseRecord
    {
        internal void Populate(SQLiteDataReader record)
        {
        }
    }
    internal class SQLiteDatabaseHandler : DatabaseHandler
    {
        private static Dictionary<System.Type, string> fieldTypes = new Dictionary<System.Type, string>()
        {
            { typeof(string), "TEXT"    },
            { typeof(int),    "INTEGER" },
            { typeof(double), "DOUBLE"  }
        };

        SQLiteConnection connection;
        internal SQLiteDatabaseHandler(string dbPath, string dbVersion) : base(dbPath, dbVersion)
        {
            connection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
        }
        internal override object GetRecord(string tableName, long recordId)
        {
            SQLiteCommand command = new SQLiteCommand($"SELECT * FROM {tableName} WHERE id = @id;", connection);
            command.Parameters.AddWithValue($"@id", recordId);
            return command.ExecuteReader();
        }
        internal override void CreateRecord(string tableName, GameEntity entity) 
        {
            using(SQLiteCommand command = new SQLiteCommand($"INSERT INTO {tableName} ( id ) VALUES ( @id );", connection))
            {
                command.Parameters.AddWithValue("@id", entity.GetLong(Field.Id));
                command.ExecuteNonQuery();
                UpdateRecord(tableName, entity);
            }
        }
        internal override void UpdateRecord(string tableName, GameEntity entity) 
        {
            Dictionary<string, object> entityFields = entity.GetSaveData();
            List<string> updateStrings = new List<string>();
            foreach(string recordKey in entityFields.Keys)
            {
                updateStrings.Add($"{recordKey} = @{recordKey}");
            }
            using(SQLiteCommand command = new SQLiteCommand($"UPDATE {tableName} SET {string.Join(", ", updateStrings)} WHERE id = @id;", connection))
            {
                foreach(KeyValuePair<string, object> recordValue in entityFields)
                {
                    command.Parameters.AddWithValue($"@{recordValue.Key}", recordValue.Value);
                }
                command.ExecuteNonQuery();
            }
        }
        internal string GetTypeString(System.Type fieldType)
        {
            if(fieldTypes.ContainsKey(fieldType))
            {
                return fieldTypes[fieldType];
            }
            return "BLOB";
        }
        internal override void InitializeTable(string tableName, Dictionary<string, (System.Type, string)> tableFields)
        {
            List<string> tableFieldStrings = new List<string>() { "id INTEGER PRIMARY KEY UNIQUE"};
            foreach(KeyValuePair<string, (System.Type, string)> field in tableFields)
            {
                string tableFieldString = $"{field.Key} {GetTypeString(field.Value.Item1)}";
                if(field.Value.Item2 != null)
                {
                    tableFieldString = $"{tableFieldString} DEFAULT {field.Value.Item2}";
                }
                tableFieldStrings.Add(tableFieldString);
            }
            string tableSchema = $"CREATE TABLE IF NOT EXISTS {tableName} ( {string.Join(", ", tableFieldStrings)} );";
            using(SQLiteCommand command = new SQLiteCommand(tableSchema, connection))
            {
                command.ExecuteNonQuery();
            }            
        }
        internal override List<DatabaseRecord> GetAllRecords(string tableName)
        { 
            List<DatabaseRecord> records = new List<DatabaseRecord>();
            using(SQLiteCommand command = new SQLiteCommand($"SELECT * FROM {tableName};", connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    DatabaseRecord record = new DatabaseRecord();
                    record.Populate(reader);
                    records.Add(record);
                }
            }
            return records;
        }
        internal override void BatchUpdateRecords(string tableName, List<GameEntity> updateQueue) 
        {
			var saveTransaction = connection.BeginTransaction();
			foreach(GameEntity objInstance in updateQueue)
			{
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
