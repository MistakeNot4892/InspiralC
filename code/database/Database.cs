using System.Collections.Generic;

namespace inspiral
{
    internal static class Database
    {
        private static string dbVersion = "3"; 
        private static Dictionary<string, DatabaseHandler> connections = new Dictionary<string, DatabaseHandler>();
        private static DatabaseHandler GetConnection(string dbPath)
        {
            if(!connections.ContainsKey(dbPath))
            {
                DatabaseHandler handler = (DatabaseHandler)(new SQLiteDatabaseHandler(dbPath, dbVersion));
                connections.Add(dbPath, handler);
                handler.Open();
                return handler;
            }
            return connections[dbPath];
        }
        internal static List<DatabaseRecord> GetAllRecords(string dbPath, string tableName, Dictionary<string, (System.Type, string)> tableFields)
        {
            DatabaseHandler handler = GetConnection(dbPath);
            handler.InitializeTable(tableName, tableFields);
            return handler.GetAllRecords(tableName);
        }

        internal static void UpdateRecord(string dbPath, string tableName, GameEntity entity)
        {
            DatabaseHandler handler = GetConnection(dbPath);
            handler.UpdateRecord(tableName, entity);
        }
        internal static void DeleteRecord(string tableName, long recordId)
        {
        }
        internal static object GetRecord(string dbPath, string tableName, long recordId)
        {
            DatabaseHandler handler = GetConnection(dbPath);
            return handler.GetRecord(tableName, recordId);
        }
        internal static void BatchUpdateRecords(string dbPath, string tableName, List<GameEntity> updateQueue)
        {
            DatabaseHandler handler = GetConnection(dbPath);
            handler.BatchUpdateRecords(tableName, updateQueue);
        }
        internal static void Exit()
        {
            foreach(KeyValuePair<string, DatabaseHandler> handler in connections)
            {
                handler.Value.Close();
            }
            connections.Clear();
        }
    }
}