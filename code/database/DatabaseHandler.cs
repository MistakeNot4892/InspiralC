using System.Collections.Generic;

namespace inspiral
{
    internal class DatabaseHandler
    {
        internal DatabaseHandler(string dbPath, string dbVersion) {}
        internal virtual List<DatabaseRecord> GetAllRecords(string tableName) { return new List<DatabaseRecord>(); }
        internal virtual void Open() {}
        internal virtual void Close() {}
        internal virtual object GetRecord(string tableName, long recordId) { return null; }
        internal virtual void InitializeTable(string tableName, Dictionary<string, (System.Type, string)> tableFields) {}
        internal virtual void UpdateRecord(string tableName, GameEntity entity) {}
        internal virtual void CreateRecord(string tableName, GameEntity entity) {}
        internal virtual void BatchUpdateRecords(string tableName, List<GameEntity> updateQueue) {}   
    }
}