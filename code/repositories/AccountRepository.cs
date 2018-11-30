using System;
using System.Data.SQLite;
using System.Collections.Generic;
using BCrypt.Net;

namespace inspiral
{

	internal class PlayerAccount
	{
		internal string userName;
		internal long id;
		internal long templateId;
		internal string passwordHash;
		internal PlayerAccount(long _id)
		{
			id = _id;
		}
		internal bool CheckPassword(string pass)
		{
			return BCrypt.Net.BCrypt.Verify(pass, passwordHash);
		}
	}

	internal class AccountRepository : GameRepository
	{
		private Dictionary<string, PlayerAccount> accounts;
		internal AccountRepository()
		{
			repoName = "accounts";
			accounts = new Dictionary<string, PlayerAccount>();
			dbTableName = "player_accounts";
			dbInsertQuery = @"INSERT INTO player_accounts ( 
				id,
				userName, 
				passwordHash,
				userTemplate
				) 
				VALUES (
					@p0,
					@p1,
					@p2,
					@p3
				);";
			dbTableSchema = @"id INTEGER PRIMARY KEY UNIQUE, 
				userName TEXT NOT NULL UNIQUE, 
				passwordHash TEXT NOT NULL,
				userTemplate INTEGER NOT NULL UNIQUE";
			}

		internal override void InstantiateFromRecord(SQLiteDataReader reader)
		{
			PlayerAccount acct = (PlayerAccount)CreateRepositoryType((long)reader["id"]);
			acct.userName =      reader["userName"].ToString();
			acct.passwordHash =  reader["passwordHash"].ToString();
			acct.templateId =    (long)reader["userTemplate"];
			accounts.Add(acct.userName, acct);
			contents.Add(acct.id, acct);
			Console.WriteLine($"Loaded account for {acct.userName} (#{acct.id}).");
		}
		internal PlayerAccount GetAccountByUser(string user)
		{
			if(accounts.ContainsKey(user))
			{
				return accounts[user];
			}
			return null;
		}

		internal PlayerAccount CreateAccount(string userName, string passwordHash)
		{
			PlayerAccount acct = (PlayerAccount)CreateNewInstance(GetUnusedIndex(), false);
			acct.userName = userName;
			acct.passwordHash = passwordHash;
			GameObjectTemplate temp = (GameObjectTemplate)Game.Templates.CreateNewInstance(false);
			acct.templateId = temp.id;
			temp.SetString(Text.FieldShortDesc, Text.Capitalize(acct.userName));
			temp.SetString(Text.FieldRoomDesc, $"{temp.GetString(Text.FieldShortDesc)} is here.");
			accounts.Add(acct.userName, acct);
			AddDatabaseEntry(acct);
			Game.Templates.AddDatabaseEntry(temp);
			return acct;
		}

		internal override Object CreateRepositoryType(long id) {
			return new PlayerAccount(id);
		}
		internal override void AddCommandParameters(SQLiteCommand command, Object instance)
		{
			PlayerAccount acct = (PlayerAccount)instance;
			command.Parameters.AddWithValue("@p0", acct.id);
			command.Parameters.AddWithValue("@p1", acct.userName);
			command.Parameters.AddWithValue("@p2", acct.passwordHash);
			command.Parameters.AddWithValue("@p3", acct.templateId);
		}
	}
}