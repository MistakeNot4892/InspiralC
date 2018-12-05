using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Diagnostics;
using BCrypt.Net;

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
				objectId
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
				objectId INTEGER NOT NULL UNIQUE";
			dbUpdateQuery = "UPDATE player_accounts SET userName = @p1, passwordHash = @p2, objectId = @p3 WHERE id = @p0;";
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader, SQLiteConnection dbConnection)
		{
			PlayerAccount acct = (PlayerAccount)CreateRepositoryType((long)reader["id"]);
			acct.userName =      reader["userName"].ToString();
			acct.passwordHash =  reader["passwordHash"].ToString();
			acct.objectId =    (long)reader["objectId"];
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
			PlayerAccount acct = (PlayerAccount)CreateNewInstance(GetUnusedIndex(), false);
			acct.userName = userName;
			acct.passwordHash = passwordHash;
			GameObject gameObj = (GameObject)Game.Objects.CreateNewInstance(false);
			acct.objectId = gameObj.id;
			gameObj.name = Text.Capitalize(acct.userName);
			gameObj.gender = Gender.GetByTerm(Gender.Androgyne);
			gameObj.AddComponent(Components.Visible);
			gameObj.SetString(Components.Visible, Text.FieldShortDesc, gameObj.name);
			gameObj.SetString(Components.Visible, Text.FieldRoomDesc, $"{gameObj.name} is here.");
			gameObj.SetString(Components.Visible, Text.FieldExaminedDesc, $"They are completely boring.");
			gameObj.AddComponent(Components.Mobile);
			gameObj.SetString(Components.Mobile, Text.FieldEnterMessage, $"{gameObj.name} enters from the $DIR.");
			gameObj.SetString(Components.Mobile, Text.FieldLeaveMessage, $"{gameObj.name} leaves to the $DIR.");
			gameObj.SetString(Components.Mobile, Text.FieldDeathMessage, $"The corpse of {gameObj.name} lies here.");
			Game.Objects.AddDatabaseEntry(gameObj);
			accounts.Add(acct.userName, acct);
			AddDatabaseEntry(acct);
			return acct;
		}
		internal override Object CreateRepositoryType(long id) 
		{
			PlayerAccount acct = new PlayerAccount(id);
			acct.roles.Add(Roles.Player);
			acct.roles.Add(Roles.Builder);
			return acct;
		}
		internal override void AddCommandParameters(SQLiteCommand command, Object instance)
		{
			PlayerAccount acct = (PlayerAccount)instance;
			command.Parameters.AddWithValue("@p0", acct.id);
			command.Parameters.AddWithValue("@p1", acct.userName);
			command.Parameters.AddWithValue("@p2", acct.passwordHash);
			command.Parameters.AddWithValue("@p3", acct.objectId);
		}
	}
}