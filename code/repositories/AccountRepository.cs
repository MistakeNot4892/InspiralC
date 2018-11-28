using System.Collections.Generic;

namespace inspiral
{
	static class AccountRepository
	{
		private static Dictionary<string, PlayerAccount> accounts;
		static AccountRepository()
		{
			accounts = new Dictionary<string, PlayerAccount>();
		}

		public static PlayerAccount CreateAccount(string username)
		{
			PlayerAccount acct = new PlayerAccount(username);
			accounts.Add(acct.username, acct);
			return acct;
		}
		public static PlayerAccount GetAccount(string username)
		{
			if(accounts.ContainsKey(username))
			{
				return accounts[username];
			}
			return null;
		}
	}
}