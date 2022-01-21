using System.Collections.Generic;

namespace inspiral
{
    internal static partial class Database
    {
        private static string dbPath = "world";
        private static string dbVersion = "3"; 
        private static DatabaseHandler? handler = null;
        private static DatabaseHandler GetConnection()
        {
            if(handler == null)
            {
                handler = (DatabaseHandler)(new SQLiteDatabaseHandler(dbPath, dbVersion));
                handler.Open();
            }
            return handler;
        }
        internal static List<Dictionary<DatabaseField, object>> GetAllRecords(GameRepository repo, string tableName, List<DatabaseField> tableFields)
        {
            DatabaseHandler handler = GetConnection();
            handler.InitializeTable(tableName, tableFields);
            return handler.GetAllRecords(tableName);
        }

        internal static void UpdateRecord(IGameEntity entity)
        {
            DatabaseHandler handler = GetConnection();
            handler.UpdateRecord(entity);
        }
        internal static void CreateRecord(GameRepository repo, IGameEntity entity)
        {
            DatabaseHandler handler = GetConnection();
            handler.CreateRecord(entity);
        }
        internal static void DeleteRecord(GameRepository repo, int recordId)
        {
        }
        internal static void BatchUpdateRecords(List<IGameEntity> updateQueue)
        {
            DatabaseHandler handler = GetConnection();
            handler.BatchUpdateRecords(updateQueue);
        }
        internal static void Exit()
        {
            if(handler != null)
            {
                handler.Close();
            }
        }
    }
}