using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{

	internal partial class Repos
	{
		internal AccountRepository Accounts = new AccountRepository();
	}
	internal class PlayerAccount : IGameEntity
	{
		internal long id = 0;
		internal string userName = "unset";
		internal long objectId = 0;
		internal string passwordHash = "unset";
		internal List<GameRole> roles = new List<GameRole>();
		internal PlayerAccount(long _id)
		{
			id = _id;
		}
		internal bool CheckPassword(string pass)
		{
			return BCrypt.Net.BCrypt.Verify(pass, passwordHash);
		}
		public Dictionary<DatabaseField, object> GetSaveData()
		{
			Dictionary<DatabaseField, object> saveData = new Dictionary<DatabaseField, object>();
			// todo
			return saveData;
		}
		public bool SetValue<T>(DatabaseField field, T newValue)
		{
			return false;
		}
		public T? GetValue<T>(DatabaseField field)
		{
			return default(T);
		}
		public void CopyFromRecord(Dictionary<DatabaseField, object> record) 
		{
			// todo
		}

	}
	internal static partial class Field
	{
		internal static DatabaseField UserName = new DatabaseField(
			"username", "",
			typeof(string), false, false);
		internal static DatabaseField PasswordHash = new DatabaseField(
			"passwordhash", "",
			typeof(string), false, false);
		internal static DatabaseField Roles = new DatabaseField(
			"roles", "",
			typeof(string), false, false);
		internal static DatabaseField ObjectId = new DatabaseField(
			"objectid", 0,
			typeof(long), false, false);
	}
	internal class AccountRepository : GameRepository
	{
		private Dictionary<string, PlayerAccount> accounts = new Dictionary<string, PlayerAccount>();
		internal override void Instantiate()
		{
			repoName = "accounts";
			dbPath = "data/accounts.sqlite";
			schemaFields = new List<DatabaseField>() { 
				Field.UserName, 
				Field.PasswordHash, 
				Field.Roles, 
				Field.ObjectId
			};
		}
		internal PlayerAccount? GetAccountByUser(string user)
		{
			if(accounts.ContainsKey(user))
			{
				return accounts[user];
			}
			return null;
		}
		internal override void InstantiateFromRecord(Dictionary<DatabaseField, object> record)
		{	
			var newAcct = CreateRepositoryType((long)record[Field.Id]);
			if(newAcct != null)
			{
				PlayerAccount acct = (PlayerAccount)newAcct;
				acct.userName =      (string)record[Field.UserName];
				acct.passwordHash =  (string)record[Field.PasswordHash];
				acct.objectId =      (long)record[Field.ObjectId];
				List<string>? roleJson = JsonConvert.DeserializeObject<List<string>>((string)record[Field.Roles]);
				if(roleJson != null)
				{
					foreach(string role in roleJson)
					{
						var getRole = Modules.Roles.GetRole(role);
						if(getRole != null)
						{
							GameRole foundRole = (GameRole)getRole;
							if(foundRole != null && !acct.roles.Contains(foundRole))
							{
								acct.roles.Add(foundRole);
							}
						}
					}
				}
				accounts.Add(acct.userName, acct);
				records.Add(acct.GetValue<long>(Field.Id), acct);
			}
		}
		internal PlayerAccount? CreateAccount(string userName, string passwordHash)
		{
			// Create the new, blank account.
			var newAcct = CreateNewInstance();
			if(newAcct != null)
			{
				PlayerAccount acct = (PlayerAccount)newAcct;
				acct.userName = userName;
				acct.passwordHash = passwordHash;

				// Create the shell the client will be piloting around, saving data to, etc.
				string myName = Text.Capitalize(acct.userName);
				GameObject gameObj = Game.Repositories.Objects.CreateFromTemplate(GlobalConfig.DefaultShellTemplate);
				gameObj.SetValue(Field.Name, myName);
				gameObj.SetValue(Field.Gender, GlobalConfig.DefaultPlayerGender);
				gameObj.SetValue(Field.Aliases, new List<string>() { myName.ToLower() });

				GenderObject genderObj = Modules.Gender.GetByTerm(gameObj.GetValue<string>(Field.Gender));
				var visComp = gameObj.GetComponent<VisibleComponent>();
				if(visComp != null)
				{
					VisibleComponent vis = (VisibleComponent)visComp;
					vis.SetValue<string>(Field.ShortDesc,    $"{gameObj.GetValue<string>(Field.Name)}");
					vis.SetValue<string>(Field.ExaminedDesc, $"and {genderObj.Is} completely uninteresting.");
				}
				Game.Repositories.Objects.QueueForUpdate(gameObj);

				acct.objectId = gameObj.GetValue<long>(Field.Id);
				
				// If the account DB is empty, give them admin roles.
				if(accounts.Count <= 0)
				{
					Game.LogError($"No accounts found, giving admin roles to {acct.userName}.");
					GameRole? role = Modules.Roles.GetRole("builder"); 
					if(role != null)
					{
						acct.roles.Add(role);
					}
					role = Modules.Roles.GetRole("administrator");
					if(role != null)
					{
						acct.roles.Add(role);
					}
				}

				// Finalize everything.
				accounts.Add(acct.userName, acct);
				Database.UpdateRecord(dbPath, $"table_{repoName}", acct);
				return acct;
			}
			return null;
		}
		internal override IGameEntity? CreateRepositoryType(long id) 
		{
			PlayerAccount acct = new PlayerAccount(id);
			GameRole? role = Modules.Roles.GetRole("player");
			if(role != null)
			{
				acct.roles.Add(role);
			}
			return acct;
		}

		internal PlayerAccount? FindAccount(string searchstring)
		{
			foreach(KeyValuePair<string, PlayerAccount> account in accounts)
			{
				if(account.Value.userName.ToLower() == searchstring ||
					$"{account.Value.GetValue<long>(Field.Id)}" == searchstring)
				{
					return account.Value;
				}
			}
			return null;
		}
	}
}

/*
		internal override void AddCommandParameters(SQLiteCommand command, System.Object instance)
		{
			PlayerAccount acct = (PlayerAccount)instance;
			command.Parameters.AddWithValue("@p0", acct.GetLong(Field.Id));
			command.Parameters.AddWithValue("@p1", acct.userName);
			command.Parameters.AddWithValue("@p2", acct.passwordHash);
			command.Parameters.AddWithValue("@p3", acct.objectId);
			List<string> roleKeys = new List<string>();
			foreach(GameRole role in acct.roles)
			{
				roleKeys.Add(role.GetValue<string>(Field.Name));
			}
			command.Parameters.AddWithValue("@p4", JsonConvert.SerializeObject(roleKeys));
		}
*/
