using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{

	internal partial class Repositories
	{
		internal AccountRepository Accounts = new AccountRepository();
	}
	internal static partial class Field
	{
	internal static DatabaseField PasswordHash = new DatabaseField(
			"passwordhash", "",
			typeof(string), false, false);
		internal static DatabaseField Roles = new DatabaseField(
			"roles", "[]",
			typeof(string), false, false);
		internal static DatabaseField ShellId = new DatabaseField(
			"shellid", 0,
			typeof(long), false, false);
	}
	internal class PlayerAccount : IGameEntity
	{
		internal Dictionary<DatabaseField, object> fields = new Dictionary<DatabaseField, object>()
		{
			{ Field.Id,           Field.Id.fieldDefault },
			{ Field.Name,         Field.Name.fieldDefault },
			{ Field.PasswordHash, Field.PasswordHash.fieldDefault },
			{ Field.Roles,        Field.Roles.fieldDefault },
			{ Field.ShellId,      Field.ShellId.fieldDefault }
		};
		internal bool CheckPassword(string pass)
		{
			return BCrypt.Net.BCrypt.Verify(pass, GetValue<string>(Field.PasswordHash));
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
	internal class AccountRepository : GameRepository
	{
		private Dictionary<string, PlayerAccount> accounts = new Dictionary<string, PlayerAccount>();
		internal override void Instantiate()
		{
			repoName = "accounts";
			dbPath = "data/accounts.sqlite";
			schemaFields = new List<DatabaseField>() { 
				Field.Id, 
				Field.Name, 
				Field.PasswordHash, 
				Field.Roles, 
				Field.ShellId
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
		internal PlayerAccount? CreateAccount(string userName, string passwordHash)
		{
			// Create the new, blank account.
			var newAcct = CreateNewInstance();
			if(newAcct != null)
			{
				PlayerAccount acct = (PlayerAccount)newAcct;
				acct.SetValue<string>(Field.Name, userName);
				acct.SetValue<string>(Field.PasswordHash, passwordHash);

				// Create the shell the client will be piloting around, saving data to, etc.
				string myName = Text.Capitalize(userName);
				GameObject gameObj = Program.Game.Repos.Objects.CreateFromTemplate(GlobalConfig.DefaultShellTemplate);
				gameObj.SetValue(Field.Gender, GlobalConfig.DefaultPlayerGender);
				gameObj.SetValue(Field.Aliases, new List<string>() { myName.ToLower() });

				GenderObject genderObj = Program.Game.Mods.Gender.GetByTerm(gameObj.GetValue<string>(Field.Gender));
				var visComp = gameObj.GetComponent<VisibleComponent>();
				if(visComp != null)
				{
					VisibleComponent vis = (VisibleComponent)visComp;
					vis.SetValue<string>(Field.ShortDesc,    $"{gameObj.GetValue<string>(Field.Name)}");
					vis.SetValue<string>(Field.ExaminedDesc, $"and {genderObj.Is} completely uninteresting.");
				}
				Program.Game.Repos.Objects.QueueForUpdate(gameObj);

				acct.SetValue<long>(Field.ShellId, gameObj.GetValue<long>(Field.Id));
				
				// If the account DB is empty, give them admin roles.
				if(accounts.Count <= 0)
				{
					Program.Game.LogError($"No accounts found, giving admin roles to {userName}.");
					
					List<GameRole> acctRoles = new List<GameRole>();
					GameRole? role = Program.Game.Mods.Roles.GetRole("builder"); 
					if(role != null)
					{
						acctRoles.Add(role);
					}
					role = Program.Game.Mods.Roles.GetRole("administrator");
					if(role != null)
					{
						acctRoles.Add(role);
					}
					acct.SetValue<List<GameRole>>(Field.Roles, acctRoles);
				}

				// Finalize everything.
				accounts.Add(userName, acct);
				Database.UpdateRecord(dbPath, $"table_{repoName}", acct);
				return acct;
			}
			return null;
		}
		internal override IGameEntity CreateNewInstance(long id)
		{
			PlayerAccount acct = (PlayerAccount)CreateRepositoryType();
			acct.SetValue<long>(Field.Id, id);
			GameRole? role = Program.Game.Mods.Roles.GetRole("player");
			if(role != null)
			{
				acct.SetValue<List<GameRole>>(Field.Roles, new List<GameRole>() { role });
			}
			return acct;
		}
		internal override IGameEntity CreateRepositoryType() 
		{
			return new PlayerAccount();
		}

		internal PlayerAccount? FindAccount(string searchstring)
		{
			foreach(KeyValuePair<string, PlayerAccount> account in accounts)
			{
				string? myName = account.Value.GetValue<string>(Field.Name);
				if(myName != null && (myName.ToLower() == searchstring ||
					$"{account.Value.GetValue<long>(Field.Id)}" == searchstring))
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
