using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal class PlayerAccount : IGameEntity
	{
		internal long id;
		internal string userName;
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
		public Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = new Dictionary<string, object>();
			// todo
			return saveData;
		}
		public bool SetValue<T>(DatabaseField field, T newValue)
		{
			return false;
		}
		public T GetValue<T>(DatabaseField field)
		{
			return default(T);
		}
		public void CopyFromRecord(Dictionary<string, object> record) 
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
		private Dictionary<string, PlayerAccount> accounts;
		internal AccountRepository()
		{
			repoName = "accounts";
			accounts = new Dictionary<string, PlayerAccount>();
			dbPath = "data/accounts.sqlite";
			schemaFields = new List<DatabaseField>() { 
				Field.UserName, 
				Field.PasswordHash, 
				Field.Roles, 
				Field.ObjectId
			};
		}
		internal PlayerAccount GetAccountByUser(string user)
		{
			if(accounts.ContainsKey(user))
			{
				return accounts[user];
			}
			return null;
		}
		internal override void InstantiateFromRecord(Dictionary<string, object> record)
		{
			PlayerAccount acct = (PlayerAccount)CreateRepositoryType((long)record[Field.Id.fieldName]);
			acct.userName =      record[Field.UserName.fieldName].ToString();
			acct.passwordHash =  record[Field.PasswordHash.fieldName].ToString();
			acct.objectId =      (long)record[Field.ObjectId.fieldName];
			foreach(string role in JsonConvert.DeserializeObject<List<string>>(record[Field.Roles.fieldName].ToString()))
			{
				GameRole foundRole = Modules.Roles.GetRole(role);
				if(foundRole != null && !acct.roles.Contains(foundRole))
				{
					acct.roles.Add(foundRole);
				}
			}
			accounts.Add(acct.userName, acct);
			records.Add(acct.GetValue<long>(Field.Id), acct);
		}
		internal PlayerAccount CreateAccount(string userName, string passwordHash)
		{
			// Create the new, blank account.
			PlayerAccount acct = (PlayerAccount)CreateNewInstance();
			acct.userName = userName;
			acct.passwordHash = passwordHash;

			// Create the shell the client will be piloting around, saving data to, etc.
			string myName = Text.Capitalize(acct.userName);
			GameObject gameObj = Game.Objects.CreateFromTemplate(GlobalConfig.DefaultShellTemplate);
			gameObj.SetValue(Field.Name, myName);
			gameObj.SetValue(Field.Gender, GlobalConfig.DefaultPlayerGender);
			gameObj.SetValue(Field.Aliases, new List<string>() { myName.ToLower() });

			GenderObject genderObj = Modules.Gender.GetByTerm(gameObj.GetValue<string>(Field.Gender));
			VisibleComponent vis = (VisibleComponent)gameObj.GetComponent<VisibleComponent>(); 
			vis.SetValue<string>(Field.ShortDesc,    $"{gameObj.GetValue<string>(Field.Name)}");
			vis.SetValue<string>(Field.ExaminedDesc, $"and {genderObj.Is} completely uninteresting.");
			Game.Objects.QueueForUpdate(gameObj);

			acct.objectId = gameObj.GetValue<long>(Field.Id);
			
			// If the account DB is empty, give them admin roles.
			if(accounts.Count <= 0)
			{
				Game.LogError($"No accounts found, giving admin roles to {acct.userName}.");
				acct.roles.Add(Modules.Roles.GetRole("builder"));
				acct.roles.Add(Modules.Roles.GetRole("administrator"));
			}

			// Finalize everything.
			accounts.Add(acct.userName, acct);
			Database.UpdateRecord(dbPath, $"table_{repoName}", acct);
			return acct;
		}
		internal override IGameEntity CreateRepositoryType(long id) 
		{
			PlayerAccount acct = new PlayerAccount(id);
			acct.roles.Add(Modules.Roles.GetRole("player"));
			return acct;
		}

		internal PlayerAccount FindAccount(string searchstring)
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
