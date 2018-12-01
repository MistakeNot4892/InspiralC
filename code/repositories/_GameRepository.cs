using System;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;

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
		internal Dictionary<long, Object> contents = new Dictionary<long, Object>();

		internal virtual void Load() {
			Console.WriteLine($"Loading {repoName}.");
			SQLiteConnection dbConnection = null;
			dbPath = $"data/{dbTableName}.sqlite";
			if(!File.Exists(dbPath))
			{
				Console.WriteLine("No database found, creating empty.");
				SQLiteConnection.CreateFile(dbPath);
				dbConnection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
				dbConnection.Open();
				using(SQLiteCommand command = new SQLiteCommand($"CREATE TABLE {dbTableName} ({dbTableSchema});", dbConnection))
				{
					try
					{
						Console.WriteLine("Created empty database.");
						command.ExecuteNonQuery();
					}
					catch(Exception e)
					{
						Console.WriteLine($"SQL exception ({repoName}): {e.ToString()}");
					}
				}
			}
			else
			{
				Console.WriteLine("Loaded.");
				dbConnection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
				dbConnection.Open();
			}
			Console.WriteLine($"Finished loading {repoName}.");
			dbConnection.Close();			
		}
		internal virtual void Initialize() 
		{
			Console.WriteLine($"Initializing {repoName}.");
			SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
			dbConnection.Open();
			using( SQLiteCommand command = new SQLiteCommand($"SELECT * FROM {dbTableName};", dbConnection))
			{
				try
				{
					SQLiteDataReader reader = command.ExecuteReader();
					while(reader.Read())
					{
						InstantiateFromRecord(reader);
					}
				}
				catch(Exception e)
				{
					Console.WriteLine($"SQL exception ({repoName}): {e.ToString()}");
				}
			}
			Console.WriteLine($"Finished initializing {repoName}.");
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
					Console.WriteLine($"SQL exception ({repoName}): {e.ToString()}");
				}
			}
			dbConnection.Close();
		}
		public virtual void DumpToConsole() { Console.WriteLine("Repo dump not implemented for this repo, sorry."); }
		internal virtual void InstantiateFromRecord(SQLiteDataReader reader) {}
		internal virtual Object CreateRepositoryType(long id) { return null; }
		internal virtual void AddCommandParameters(SQLiteCommand command, Object instance) {}
	}
}