using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace inspiral
{
	internal static partial class Repositories
	{
		internal static Dictionary<System.Type, GameRepository> AllRepositories = new Dictionary<System.Type, GameRepository>(); 
		internal static GameRepository GetRepository<T>()
		{
			return AllRepositories[typeof(T)];
		}
		internal static void Populate()
		{
			Game.LogError($"Creating repositories.");
			foreach(GameRepository repo in Game.InstantiateSubclasses<GameRepository>())
			{
				AllRepositories.Add(repo.GetType(), repo);
			}

			Game.LogError($"Populating repositories.");
			foreach(KeyValuePair<System.Type, GameRepository> repo in AllRepositories)
			{
				Game.LogError($"Populating {repo.Value.repoName}.");
				repo.Value.Populate();
				Game.LogError($"Finished initial load of {repo.Value.loadingEntities.Count} record(s) from {repo.Value.repoName}.");
				Task.Delay(1000);
			}
		}
		internal static void Initialize()
		{
			// TODO sort repositories by priority value for init ordering
			Game.LogError($"Initializing repositories.");
			foreach(KeyValuePair<System.Type, GameRepository> repo in AllRepositories)
			{
				Game.LogError($"Initializing {repo.Value.repoName}.");
				repo.Value.Initialize();
				Game.LogError($"Finished initialization of {repo.Value.loadingEntities.Count} record(s) from {repo.Value.repoName}.");
				Task.Delay(1000);
			}
		}
		internal static void PostInitialize()
		{
			Game.LogError($"Post-initializing repositories.");
			foreach(KeyValuePair<System.Type, GameRepository> repo in AllRepositories)
			{
				Game.LogError($"Post-initializing {repo.Value.repoName}.");
				repo.Value.PostInitialize();
				Game.LogError($"Finished post-initialization of {repo.Value.loadingEntities.Count} record(s) from {repo.Value.repoName}.");
				Task.Delay(1000);
			}
		}
		internal static void ExitRepos()
		{
			foreach(KeyValuePair<System.Type, GameRepository> repo in AllRepositories)
			{
				repo.Value.Exit();
			}
		}

	}
	class GameRepository
	{

		internal string repoName = "repository";
		internal string dbPath = "data/gamedata.sqlite";
		internal List<DatabaseField>? schemaFields;
		internal Dictionary<IGameEntity, Dictionary<DatabaseField, object>> loadingEntities = new Dictionary<IGameEntity, Dictionary<DatabaseField, object>>();
		internal Dictionary<ulong, IGameEntity> records = new Dictionary<ulong, IGameEntity>();
		private List<IGameEntity> updateQueue = new List<IGameEntity>();
		private bool killUpdateProcess = false;

		public GameRepository() { }
		internal void QueueForUpdate(GameComponent comp)
		{
			var parent = comp.GetParent();
			if(parent != null)
			{
				QueueForUpdate(parent);
			}
		}

		internal void QueueForUpdate(IGameEntity obj)
		{
			if(!updateQueue.Contains(obj) && Game.InitComplete)
			{
				updateQueue.Add(obj);
			}
		}
		internal virtual string? GetAdditionalClassInfo(Dictionary<DatabaseField, object> record)
		{
			return null;
		}
		internal virtual void Populate()
		{
			if(schemaFields != null)
			{
				foreach(Dictionary<DatabaseField, object> record in Database.GetAllRecords(dbPath, $"table_{repoName}", schemaFields))
				{
					ulong? eId = (ulong)record[Field.Id];
					if(eId == null)
					{
						continue;
					}
					IGameEntity newEntity = CreateNewInstance((ulong)eId, GetAdditionalClassInfo(record));
					loadingEntities.Add(newEntity, record);
				}
			}
		}
		internal virtual void Initialize() 
		{
			Game.LogError($"- Initializing {loadingEntities.Count} record(s) in {repoName}.");
			foreach(KeyValuePair<IGameEntity, Dictionary<DatabaseField, object>> loadingEntity in loadingEntities)
			{
				loadingEntity.Key.CopyFromRecord(loadingEntity.Value);
			}
			Game.LogError($"- Finished initializing {repoName}.");
		}
		internal virtual void PostInitialize()
		{
			Task.Run(() => DoPeriodicDatabaseUpdate() );
			loadingEntities.Clear();
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

		internal IGameEntity? GetById(ulong? id)
		{
			if(id != null && records.ContainsKey((ulong)id))
			{
				return records[(ulong)id];
			}
			return null;
		}
		internal ulong GetUnusedIndex()
		{
			// TODO scrape for unused indices
			return (ulong)records.Count+1;
		}
		internal virtual IGameEntity CreateNewInstance()
		{
			return CreateNewInstance(GetUnusedIndex(), null);
		}
		internal virtual IGameEntity CreateNewInstance(ulong id)
		{
			return CreateNewInstance(id, null);
		}
		internal virtual IGameEntity CreateNewInstance(string? additionalClassInfo)
		{
			return CreateNewInstance(GetUnusedIndex(), additionalClassInfo);
		}
		internal virtual IGameEntity CreateNewInstance(ulong id, string? additionalClassInfo)
		{
			IGameEntity newInstance = CreateRepositoryType(additionalClassInfo);
			newInstance.SetValue<ulong>(Field.Id, id);
			return newInstance;
		}
		public virtual void DumpToConsole() 
		{
			Game.LogError("Repo dump not implemented for this repo, sorry.");
		}
		internal virtual IGameEntity CreateRepositoryType(string? additionalClassInfo)
		{
			return new GameObject();
		}
	}
}