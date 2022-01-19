using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace inspiral
{
	internal partial class Repositories
	{
		internal List<GameRepository> AllRepositories = new List<GameRepository>(); 

		internal void Populate()
		{
			Program.Game.LogError($"Creating repositories.");
			foreach(var t in (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(GameRepository))
				select assemblyType))
			{
				var repo = System.Activator.CreateInstance(t);
				if(repo != null)
				{
					AllRepositories.Add((GameRepository)repo);
				}
			}

			Program.Game.LogError($"Populating repositories.");
			foreach(GameRepository repo in AllRepositories)
			{
				Program.Game.LogError($"Populating {repo.repoName}.");
				repo.Populate();
				Program.Game.LogError($"Finished initial load of {repo.loadingEntities.Count} record(s) from {repo.repoName}.");
				Task.Delay(1000);
			}
		}
		internal void Initialize()
		{
			// TODO sort repositories by priority value for init ordering
			Program.Game.LogError($"Initializing repositories.");
			foreach(GameRepository repo in AllRepositories)
			{
				Program.Game.LogError($"Initializing {repo.repoName}.");
				repo.Initialize();
				Program.Game.LogError($"Finished initialization of {repo.loadingEntities.Count} record(s) from {repo.repoName}.");
				Task.Delay(1000);
			}
		}
		internal void PostInitialize()
		{
			Program.Game.LogError($"Post-initializing repositories.");
			foreach(GameRepository repo in AllRepositories)
			{
				Program.Game.LogError($"Post-initializing {repo.repoName}.");
				repo.PostInitialize();
				Program.Game.LogError($"Finished post-initialization of {repo.loadingEntities.Count} record(s) from {repo.repoName}.");
				Task.Delay(1000);
			}
		}
		internal void ExitRepos()
		{
			foreach(GameRepository repo in AllRepositories)
			{
				repo.Exit();
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
			if(!updateQueue.Contains(obj) && Program.Game.InitComplete)
			{
				updateQueue.Add(obj);
			}
		}
		internal virtual void Populate()
		{
			if(schemaFields != null)
			{
				foreach(Dictionary<DatabaseField, object> record in Database.GetAllRecords(dbPath, $"table_{repoName}", schemaFields))
				{
					int? eId = (int)record[Field.Id];
					if(eId == null)
					{
						continue;
					}
					IGameEntity newEntity = CreateNewInstance((ulong)eId);
					loadingEntities.Add(newEntity, record);
				}
			}
		}
		internal virtual void Initialize() 
		{
			Program.Game.LogError($"- Initializing {loadingEntities.Count} record(s) in {repoName}.");
			foreach(KeyValuePair<IGameEntity, Dictionary<DatabaseField, object>> loadingEntity in loadingEntities)
			{
				loadingEntity.Key.CopyFromRecord(loadingEntity.Value);
			}
			Program.Game.LogError($"- Finished initializing {repoName}.");
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
			Program.Game.LogError($"- Starting periodic save thread for {repoName}.");
			while(!killUpdateProcess)
			{
				if(updateQueue.Count > 0)
				{
					Database.BatchUpdateRecords(dbPath, $"table_{repoName}", updateQueue);
				}
				Thread.Sleep(5000);
			}
			Program.Game.LogError($"- Terminating periodic save thread for {repoName}.");
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
			return CreateNewInstance(GetUnusedIndex());
		}
		internal virtual IGameEntity CreateNewInstance(ulong id)
		{
			IGameEntity newInstance = CreateRepositoryType();
			newInstance.SetValue<ulong>(Field.Id, id);
			return newInstance;
		}
		public virtual void DumpToConsole() 
		{
			Program.Game.LogError("Repo dump not implemented for this repo, sorry.");
		}
		internal virtual IGameEntity CreateRepositoryType() 
		{
			return new GameObject();
		}
	}
}