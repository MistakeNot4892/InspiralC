using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// It's a repo repo!
namespace inspiral
{
	internal static class Game
	{
		internal static ClientRepository Clients;
		internal static AccountRepository Accounts;
		internal static ObjectRepository Objects;
		internal static TemplateRepository Templates;
		internal static RoomRepository Rooms;
		
		static Game()
		{
			Clients =   new ClientRepository();
			Accounts =  new AccountRepository();
			Rooms =     new RoomRepository();
			Objects =   new ObjectRepository();
			Templates = new TemplateRepository();
		}
		internal static void Load()
		{
			Accounts.Load();
			Rooms.Load();
			Templates.Load(); 
			Objects.Load();
		}
		internal static void Initialize() 
		{
			Accounts.Initialize();
			Rooms.Initialize();
			Templates.Initialize(); 
			Objects.Initialize();

			Accounts.PostInitialize();
			Rooms.PostInitialize();
			Templates.PostInitialize(); 
			Objects.PostInitialize();
		}
	}
}
