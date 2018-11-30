using System;
using System.Collections.Generic;
using BCrypt.Net;

namespace inspiral
{
	public class PlayerAccount
	{
		public string username;
		private string passwordHash;
		public PlayerAccount(string _username, string _passwordHash)
		{
			username = _username;
			passwordHash = _passwordHash;
		}
		public bool CheckPassword(string pass)
		{
			return BCrypt.Net.BCrypt.Verify(pass, passwordHash);
		}
	}
}