using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal class PlayerAccount : GameEntity
	{
		internal string userName;
		internal long objectId;
		internal string passwordHash;
		internal List<GameRole> roles = new List<GameRole>();

		internal PlayerAccount(long _id) : base(_id)
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
			dbPath = "data/accounts.sqlite";
			schemaFields = new Dictionary<string, (System.Type, string)>() 
			{
				{"userName",     (typeof(string), "''")}, 
				{"passwordHash", (typeof(string), "''")},
				{"roles",        (typeof(string), "''")},
				{"objectId",     (typeof(int),    "0")}
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
		internal override void InstantiateFromRecord(DatabaseRecord record)
		{
			PlayerAccount acct = (PlayerAccount)CreateRepositoryType((long)record.fields["id"]);
			acct.userName =      record.fields["userName"].ToString();
			acct.passwordHash =  record.fields["passwordHash"].ToString();
			acct.objectId =      (long)record.fields["objectId"];
			foreach(string role in JsonConvert.DeserializeObject<List<string>>(record.fields["roles"].ToString()))
			{
				GameRole foundRole = Modules.Roles.GetRole(role);
				if(foundRole != null && !acct.roles.Contains(foundRole))
				{
					acct.roles.Add(foundRole);
				}
			}
			accounts.Add(acct.userName, acct);
			records.Add(acct.id, acct);
		}
		internal PlayerAccount CreateAccount(string userName, string passwordHash)
		{
			// Create the new, blank account.
			PlayerAccount acct = (PlayerAccount)CreateNewInstance();
			acct.userName = userName;
			acct.passwordHash = passwordHash;

			// Create the shell the client will be piloting around, saving data to, etc.
			GameObject gameObj = Modules.Templates.Instantiate(GlobalConfig.DefaultShellTemplate);
			gameObj.name = Text.Capitalize(acct.userName);
			gameObj.gender = Modules.Gender.GetByTerm(GlobalConfig.DefaultPlayerGender);
			gameObj.aliases = new List<string>() { gameObj.name.ToLower() };

			VisibleComponent vis = (VisibleComponent)gameObj.GetComponent<VisibleComponent>(); 
			vis.SetValue(Text.FieldShortDesc,    $"{gameObj.name}");
			vis.SetValue(Text.FieldExaminedDesc, $"and {gameObj.gender.Is} completely uninteresting.");
			Game.Objects.QueueForUpdate(gameObj);

			acct.objectId = gameObj.id;
			
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
		internal override GameEntity CreateRepositoryType(long id) 
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
					$"{account.Value.id}" == searchstring)
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
			command.Parameters.AddWithValue("@p0", acct.id);
			command.Parameters.AddWithValue("@p1", acct.userName);
			command.Parameters.AddWithValue("@p2", acct.passwordHash);
			command.Parameters.AddWithValue("@p3", acct.objectId);
			List<string> roleKeys = new List<string>();
			foreach(GameRole role in acct.roles)
			{
				roleKeys.Add(role.name);
			}
			command.Parameters.AddWithValue("@p4", JsonConvert.SerializeObject(roleKeys));
		}
*/
