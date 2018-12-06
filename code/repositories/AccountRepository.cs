using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Diagnostics;
using BCrypt.Net;
using Newtonsoft.Json;

namespace inspiral
{
	internal class PlayerAccount
	{
		internal string userName;
		internal long id;
		internal long objectId;
		internal string passwordHash;
		internal List<GameRole> roles = new List<GameRole>();

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
				objectId,
				roles
				) 
				VALUES (
					@p0,
					@p1,
					@p2,
					@p3,
					@p4
				);";
			dbTableSchema = @"id INTEGER PRIMARY KEY UNIQUE, 
				userName TEXT NOT NULL UNIQUE, 
				passwordHash TEXT NOT NULL,
				objectId INTEGER NOT NULL UNIQUE,
				roles TEXT DEFAULT ''";
			dbUpdateQuery = "UPDATE player_accounts SET userName = @p1, passwordHash = @p2, objectId = @p3, roles = @p4 WHERE id = @p0;";
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader, SQLiteConnection dbConnection)
		{
			PlayerAccount acct = (PlayerAccount)CreateRepositoryType((long)reader["id"]);
			acct.userName =      reader["userName"].ToString();
			acct.passwordHash =  reader["passwordHash"].ToString();
			acct.objectId =    (long)reader["objectId"];
			foreach(string role in JsonConvert.DeserializeObject<List<string>>(reader["roles"].ToString()))
			{
				GameRole foundRole = Roles.GetRole(role);
				if(foundRole != null && !acct.roles.Contains(foundRole))
				{
					acct.roles.Add(foundRole);
				}
			}
			accounts.Add(acct.userName, acct);
			contents.Add(acct.id, acct);
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

			// Create the new, blank account.
			PlayerAccount acct = (PlayerAccount)CreateNewInstance(GetUnusedIndex(), false);
			acct.userName = userName;
			acct.passwordHash = passwordHash;

			// Create the shell the client will be piloting around, saving data to, etc.
			GameObject gameObj = (GameObject)Game.Objects.CreateMob(Text.Capitalize(acct.userName));
			acct.objectId = gameObj.id;
			
			// If the account DB is empty, give them admin roles.
			if(accounts.Count <= 0)
			{
				Debug.WriteLine($"No accounts found, giving admin roles to {acct.userName}.");
				acct.roles.Add(Roles.Builder);
				acct.roles.Add(Roles.Administrator);
			}

			// Finalize everything.
			accounts.Add(acct.userName, acct);
			AddDatabaseEntry(acct);
			return acct;
		}
		internal override Object CreateRepositoryType(long id) 
		{
			PlayerAccount acct = new PlayerAccount(id);
			acct.roles.Add(Roles.Player);
			return acct;
		}
		internal override void AddCommandParameters(SQLiteCommand command, Object instance)
		{
			PlayerAccount acct = (PlayerAccount)instance;
			command.Parameters.AddWithValue("@p0", acct.id);
			command.Parameters.AddWithValue("@p1", acct.userName);
			command.Parameters.AddWithValue("@p2", acct.passwordHash);
			command.Parameters.AddWithValue("@p3", acct.objectId);
			List<string> roleKeys = new List<string>();
			foreach(GameRole role in acct.roles)
			{
				roleKeys.Add(role.Name);
			}
			command.Parameters.AddWithValue("@p4", JsonConvert.SerializeObject(roleKeys));
		}

		internal PlayerAccount FindAccount(string searchstring)
		{
			foreach(KeyValuePair<string, PlayerAccount> account in accounts)
			{
				if(account.Value.userName.ToLower() == searchstring ||
					$"{account.Value.id}" == searchstring)
				{
					return account.Value;
				}
			}
			return null;
		}
	}
}