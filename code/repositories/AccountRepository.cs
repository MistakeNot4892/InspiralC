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
			typeof(int), false, false);
	}
	internal class PlayerAccount : IGameEntity
	{
		internal void AddRole(GameRole role)
		{
			if(!roles.Contains(role))
			{
				roles.Add(role);
			}
		}
		internal void AddRole(string roleName)
		{
			GameRole? role = Program.Game.Mods.Roles.GetRole(roleName);
			if(role != null)
			{
				AddRole(role);
			}
		}
		internal List<GameRole> roles = new List<GameRole>();
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
			fields = record;
			BuildReferenceLists();
		}
		internal void BuildReferenceLists()
		{
			List<string>? roleNames = JsonConvert.DeserializeObject<List<string>>((string)fields[Field.Roles]);
			if(roleNames != null)
			{
				foreach(string roleName in roleNames)
				{
					GameRole? role = Program.Game.Mods.Roles.GetRole(roleName);
					if(role != null)
					{
						roles.Add(role);
					}
				}
			}
		}
		public Dictionary<DatabaseField, object> GetSaveData()
		{
			List<string> roleNames = new List<string>();
			foreach(GameRole role in roles)
			{
				roleNames.Add(role.name);
			}
			fields[Field.Roles] = JsonConvert.SerializeObject(roleNames);
			return fields;
		}

	}
	internal class AccountRepository : GameRepository
	{
		private Dictionary<string, PlayerAccount> accounts = new Dictionary<string, PlayerAccount>();
		public AccountRepository()
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

				acct.SetValue<ulong>(Field.ShellId, gameObj.GetValue<ulong>(Field.Id));
				
				// If the account DB is empty, give them admin roles.
				acct.AddRole("player");
				if(accounts.Count <= 0)
				{
					Program.Game.LogError($"No accounts found, giving admin roles to {userName}.");
					acct.AddRole("builder"); 
					acct.AddRole("administrator");
				}

				// Finalize everything.
				accounts.Add(userName, acct);
				Database.UpdateRecord(dbPath, $"table_{repoName}", acct);
				return acct;
			}
			return null;
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
					$"{account.Value.GetValue<ulong>(Field.Id)}" == searchstring))
				{
					return account.Value;
				}
			}
			return null;
		}
	}
}
