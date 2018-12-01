using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// It's a repo repo!
namespace inspiral
{
	internal static class Game
	{
		internal static AccountRepository Accounts;
		internal static ObjectRepository Objects;
		
		static Game()
		{
			Accounts =  new AccountRepository();
			Objects =   new ObjectRepository();
		}
		internal static void Load()
		{
			Accounts.Load();
			Objects.Load();
		}
		internal static void Initialize() 
		{
			Accounts.Initialize();
			Objects.Initialize();

			Accounts.PostInitialize();
			Objects.PostInitialize();
		}
	}
}
