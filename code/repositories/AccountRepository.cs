using System;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;

namespace inspiral
{
	static class AccountRepository
	{
		private static string dbPath = "data/players.sqlite";
		private static string dbVersion = "3";
		private static Dictionary<string, PlayerAccount> accounts;
		static AccountRepository()
		{
			accounts = new Dictionary<string, PlayerAccount>();
			SQLiteConnection dbConnection = null;
			if(!File.Exists(dbPath))
			{
				SQLiteConnection.CreateFile(dbPath);
				dbConnection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
				dbConnection.Open();
				using(SQLiteCommand command = new SQLiteCommand("CREATE TABLE player_accounts (userId INTEGER PRIMARY KEY AUTOINCREMENT, userName TEXT NOT NULL UNIQUE, passwordHash TEXT NOT NULL, descString TEXT DEFAULT 'A generic person.');", dbConnection))
				{
					try
					{
						command.ExecuteNonQuery();
					}
					catch(Exception e)
					{
						Console.WriteLine($"SQL exception: {e.ToString()}");
					}
				}
			}
			else
			{
				dbConnection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
				dbConnection.Open();
			}
			// Load records.
			using( SQLiteCommand command = new SQLiteCommand("SELECT * FROM player_accounts;", dbConnection))
			{
				try
				{
					SQLiteDataReader reader = command.ExecuteReader();
					while(reader.Read())
					{
						string userName = reader["userName"].ToString();
						accounts.Add(userName, new PlayerAccount(userName, reader["passwordHash"].ToString()));
						Console.WriteLine($"Loaded account for {userName}.");
					}
				}
				catch(Exception e)
				{
					Console.WriteLine($"SQL exception: {e.ToString()}");
				}
			}
			dbConnection.Close();
		}

		public static PlayerAccount CreateAccount(string userName, string passwordHash)
		{
			SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={dbPath};Version={dbVersion};");
			dbConnection.Open();
			using(SQLiteCommand command = new SQLiteCommand("INSERT INTO player_accounts ( userName, passwordHash ) VALUES ( @p1, @p2 );", dbConnection))
			{
				try
				{
					userName = userName.ToLower();
					command.Parameters.AddWithValue("@p1", userName);
					command.Parameters.AddWithValue("@p2", passwordHash);
					command.ExecuteNonQuery();
				}
				catch(Exception e)
				{
					Console.WriteLine($"Account SQL exception: {e.ToString()}");
				}
			}
			dbConnection.Close();
			Console.WriteLine($"Account created for {userName}.");
			PlayerAccount acct = new PlayerAccount(userName, passwordHash);
			accounts.Add(acct.username, acct);
			return acct;
		}
		public static PlayerAccount GetAccount(string userName)
		{
			userName = userName.ToLower();
			if(accounts.ContainsKey(userName))
			{
				return accounts[userName];
			}
			return null;
		}
	}
}