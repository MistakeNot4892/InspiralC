using System;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace inspiral
{
	class GameRepository
	{

		internal SQLiteConnection dbConnection;
		internal string repoName = "unnamed repository";
		internal string dbPath;
		internal string dbVersion = "3";
		internal string dbTableSchema;
		internal string dbTableName;
		internal string dbInsertQuery;
		internal string dbUpdateQuery;
		internal Dictionary<long, Object> contents = new Dictionary<long, Object>();
		private List<Object> updateQueue = new List<Object>();
		private bool killUpdateProcess = false;

		internal virtual void QueueForUpdate(Object obj)
		{
			if(!updateQueue.Contains(obj))
			{
				updateQueue.Add(obj);
			}
		}

		internal virtual void Load() {
			Debug.WriteLine($"Loading {repoName}.");
			dbPath = $"data/{dbTableName}.sqlite";
			if(!File.Exists(dbPath))
			{
				Debug.WriteLine("No database found, creating empty.");
				SQLiteConnection.CreateFile(dbPath);
			}

			dbConnection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
			dbConnection.Open();
			using(SQLiteCommand command = new SQLiteCommand($"CREATE TABLE IF NOT EXISTS {dbTableName} ({dbTableSchema});", dbConnection))
			{
				try
				{
					command.ExecuteNonQuery();
				}
				catch(Exception e)
				{
					Debug.WriteLine($"SQL exception 1 ({repoName}): {e.ToString()} - entire query is [CREATE TABLE IF NOT EXISTS {dbTableName} ({dbTableSchema});]");
				}
			}
			HandleSecondarySQLInitialization(dbConnection);
			Debug.WriteLine($"Finished loading {repoName}.");
		}
		internal virtual void Initialize() 
		{
			Debug.WriteLine($"Initializing {repoName}.");
			SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
			dbConnection.Open();
			using( SQLiteCommand command = new SQLiteCommand($"SELECT * FROM {dbTableName};", dbConnection))
			{
				try
				{
					SQLiteDataReader reader = command.ExecuteReader();
					while(reader.Read())
					{
						InstantiateFromRecord(reader, dbConnection);
					}
				}
				catch(Exception e)
				{
					Debug.WriteLine($"SQL exception 2 ({repoName}): {e.ToString()} - entire query is [SELECT * FROM {dbTableName};]");
				}
			}
			Task.Run(() => DoPeriodicDatabaseUpdate() );
			Debug.WriteLine($"Finished initializing {repoName}.");
		}

		internal void DoPeriodicDatabaseUpdate()
		{
			Debug.WriteLine($"Starting periodic save thread for {repoName}.");
			while(!killUpdateProcess)
			{
				if(updateQueue.Count > 0)
				{
					int saveCount = 500;
					if(saveCount > updateQueue.Count)
					{
						saveCount = updateQueue.Count;
					}
					for(int i = 0;i<saveCount;i++)
					{
						Object saving = updateQueue[0];
						updateQueue.Remove(saving);
						SaveObject(saving);
					}
				}
				Thread.Sleep(5000);
			}
			Debug.WriteLine($"Terminating periodic save thread for {repoName}.");
		}

		internal virtual void PostInitialize() {}
		internal Object Get(long id)
		{
			if(contents.ContainsKey(id))
			{
				return contents[id];
			}
			return null;
		}
		internal long GetUnusedIndex()
		{
			return (long)contents.Count+1;
		}
		internal virtual Object CreateNewInstance(bool addToDatabase)
		{
			return CreateNewInstance(GetUnusedIndex(), addToDatabase);
		}
		internal virtual Object CreateNewInstance(long id, bool addToDatabase)
		{
			Object newInstance = CreateRepositoryType(id);
			contents.Add(id, newInstance);
			if(addToDatabase)
			{
				AddDatabaseEntry(newInstance);
			}
			return Get(id);
		}
		internal virtual void AddDatabaseEntry(Object newInstance)
		{
			using(SQLiteCommand command = new SQLiteCommand(dbInsertQuery, dbConnection))
			{
				try
				{
					AddCommandParameters(command, newInstance);
					command.ExecuteNonQuery();
				}
				catch(Exception e)
				{
					Debug.WriteLine($"SQL exception 3 ({repoName}): {e.ToString()} - enter query is [{dbInsertQuery}]");
				}
			}
			HandleAdditionalSQLInsertion(newInstance, dbConnection);
		}
		public void SaveObject(Object objInstance)
		{
			using(SQLiteCommand command = new SQLiteCommand(dbUpdateQuery, dbConnection))
			{
				try
				{
					AddCommandParameters(command, objInstance);
					command.ExecuteNonQuery();
				}
				catch(Exception e)
				{
					Debug.WriteLine($"SQL exception 6 ({repoName}): {e.ToString()} - enter query is [{dbUpdateQuery}]");
				}
			}
			HandleAdditionalObjectSave(objInstance, dbConnection);
		}
		public virtual void HandleAdditionalObjectSave(Object objInstance, SQLiteConnection dbConnection) {}
		public virtual void HandleAdditionalSQLInsertion(Object newInstance, SQLiteConnection dbConnection) {}
		public virtual void DumpToConsole() { Debug.WriteLine("Repo dump not implemented for this repo, sorry."); }
		internal virtual void InstantiateFromRecord(SQLiteDataReader reader, SQLiteConnection dbConnection) {}
		internal virtual Object CreateRepositoryType(long id) { return null; }
		internal virtual void AddCommandParameters(SQLiteCommand command, Object instance) {}
		internal virtual void HandleSecondarySQLInitialization(SQLiteConnection dbConnection) {}
		internal virtual void Exit() 
		{
			killUpdateProcess = true;
			dbConnection.Close();
		}
	}
}