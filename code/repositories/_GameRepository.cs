using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace inspiral
{
	internal static partial class Repos
	{
		internal static List<GameRepository> s_repos = new List<GameRepository>(); 
		internal static void InstantiateRepos()
		{
			Game.LogError($"Instantiating repositories.");
			foreach(var t in (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameRepository))
				select assemblyType))
			{
				Game.LogError($"- Creating repository {t}.");
				GameRepository repo = (GameRepository)System.Activator.CreateInstance(t);
				s_repos.Add(repo);
			}
			// sort repositories by priority value for init ordering
		}
		internal static void LoadRepos()
		{
			foreach(GameRepository repo in s_repos)
			{
				repo.Load();
			}
		}
		internal static void InitializeRepos()
		{
			foreach(GameRepository repo in s_repos)
			{
				repo.Initialize();
			}
		}
		internal static void PostInitializeRepos()
		{
			foreach(GameRepository repo in s_repos)
			{
				repo.PostInitialize();
			}
		}
		internal static void ExitRepos()
		{
			foreach(GameRepository repo in s_repos)
			{
				repo.Exit();
			}
		}

	}
	class GameRepository
	{

		internal string repoName = "repository";
		internal string dbPath = "data/gamedata.sqlite";
		internal List<DatabaseField> schemaFields;
		internal Dictionary<long, IGameEntity> records = new Dictionary<long, IGameEntity>();
		private List<IGameEntity> updateQueue = new List<IGameEntity>();
		private bool killUpdateProcess = false;
		internal GameRepository() { Instantiate(); }
		internal virtual void Instantiate() {}

		internal virtual void QueueForUpdate(IGameEntity obj)
		{
			if(!updateQueue.Contains(obj) && Game.InitComplete)
			{
				updateQueue.Add(obj);
			}
		}
		internal virtual void InstantiateFromRecord(Dictionary<DatabaseField, object> record) {}

		internal virtual void Load() {
			Game.LogError($"- Loading {repoName}.");
			foreach(Dictionary<DatabaseField, object> record in Database.GetAllRecords(dbPath, $"table_{repoName}", schemaFields))
			{
				InstantiateFromRecord(record);
			}
			Game.LogError($"- Finished loading {repoName}.");
		}
		internal virtual void Initialize() 
		{
			Game.LogError($"- Initializing {repoName}.");
			Task.Run(() => DoPeriodicDatabaseUpdate() );
			Game.LogError($"- Finished initializing {repoName}.");
		}
		internal virtual void Exit()
		{
			killUpdateProcess = true; 
		}
		internal void DoPeriodicDatabaseUpdate()
		{
			Game.LogError($"- Starting periodic save thread for {repoName}.");
			while(!killUpdateProcess)
			{
				if(updateQueue.Count > 0)
				{
					Database.BatchUpdateRecords(dbPath, $"table_{repoName}", updateQueue);
				}
				Thread.Sleep(5000);
			}
			Game.LogError($"- Terminating periodic save thread for {repoName}.");
		}

		internal virtual void PostInitialize() {}
		internal IGameEntity GetByID(long id)
		{
			if(records.ContainsKey(id))
			{
				return records[id];
			}
			return null;
		}
		internal long GetUnusedIndex()
		{
			// TODO scrape for unused indices
			return (long)records.Count+1;
		}
		internal virtual IGameEntity CreateNewInstance()
		{
			return CreateNewInstance(GetUnusedIndex());
		}
		internal virtual IGameEntity CreateNewInstance(long id)
		{
			IGameEntity newInstance = CreateRepositoryType(id);
			records.Add(id, newInstance);
			return GetByID(id);
		}
		public virtual void DumpToConsole() 
		{
			Game.LogError("Repo dump not implemented for this repo, sorry.");
		}
		internal virtual IGameEntity CreateRepositoryType(long id) 
		{
			return null;
		}
	}
}