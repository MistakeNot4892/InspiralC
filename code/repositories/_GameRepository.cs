using System;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Diagnostics;

namespace inspiral
{
	class GameRepository
	{
		internal string repoName = "unnamed repository";
		internal string dbPath;
		internal string dbVersion = "3";
		internal string dbTableSchema;
		internal string dbTableName;
		internal string dbInsertQuery;
		internal string dbUpdateQuery;
		internal Dictionary<long, Object> contents = new Dictionary<long, Object>();

		internal virtual void Load() {
			Debug.WriteLine($"Loading {repoName}.");
			SQLiteConnection dbConnection = null;
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
			dbConnection.Close();			
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
			Debug.WriteLine($"Finished initializing {repoName}.");
			dbConnection.Close();
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
			SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
			dbConnection.Open();
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
			dbConnection.Close();
		}
		public void SaveObject(Object objInstance)
		{
			SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
			dbConnection.Open();
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
			dbConnection.Close();
		}
		public virtual void HandleAdditionalObjectSave(Object objInstance, SQLiteConnection dbConnection) {}
		public virtual void HandleAdditionalSQLInsertion(Object newInstance, SQLiteConnection dbConnection) {}
		public virtual void DumpToConsole() { Debug.WriteLine("Repo dump not implemented for this repo, sorry."); }
		internal virtual void InstantiateFromRecord(SQLiteDataReader reader, SQLiteConnection dbConnection) {}
		internal virtual Object CreateRepositoryType(long id) { return null; }
		internal virtual void AddCommandParameters(SQLiteCommand command, Object instance) {}
		internal virtual void HandleSecondarySQLInitialization(SQLiteConnection dbConnection) {}
	}
}