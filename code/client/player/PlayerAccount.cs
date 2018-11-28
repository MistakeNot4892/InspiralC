using System.Collections.Generic;

namespace inspiral
{
	public class PlayerAccount
	{
		public string username;
		public PlayerAccount(string _username)
		{
			username = _username;
		}
		public bool CheckPassword(string pass)
		{
			return true;
		}
	}
}