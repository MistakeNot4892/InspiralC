using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace inspiral
{
	internal partial class Repositories
	{
		private List<GameRepository> s_repos = new List<GameRepository>(); 
		public List<GameRepository> AllRepositories {
			get { return s_repos; }
			set { s_repos = value; }
		}
		internal void Populate()
		{
			foreach(GameRepository repo in s_repos)
			{
				repo.Populate();
			}
		}
		internal void Initialize()
		{
			// TODOL sort repositories by priority value for init ordering
			foreach(GameRepository repo in s_repos)
			{
				repo.Initialize();
			}
		}
		internal void PostInitialize()
		{
			foreach(GameRepository repo in s_repos)
			{
				repo.PostInitialize();
			}
		}
		internal void ExitRepos()
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
		internal List<DatabaseField>? schemaFields;
		internal Dictionary<long, IGameEntity> records = new Dictionary<long, IGameEntity>();
		private List<IGameEntity> updateQueue = new List<IGameEntity>();
		private bool killUpdateProcess = false;
		internal GameRepository() { Instantiate(); }
		internal virtual void Instantiate() {}

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
			Program.Game.LogError($"- Populating {repoName}.");
			if(schemaFields != null)
			{
				foreach(Dictionary<DatabaseField, object> record in Database.GetAllRecords(dbPath, $"table_{repoName}", schemaFields))
				{
					long? eId = (long)record[Field.Id];
					if(eId == null)
					{
						continue;
					}
					IGameEntity newEntity = CreateNewInstance((long)eId);
				}
			}
			Program.Game.LogError($"- Finished loading {repoName}.");
		}
		internal virtual void Initialize() 
		{
			Program.Game.LogError($"- Initializing {repoName}.");
			Program.Game.LogError($"- Finished initializing {repoName}.");
		}
		internal virtual void PostInitialize()
		{
			Task.Run(() => DoPeriodicDatabaseUpdate() );
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

		internal IGameEntity? GetById(long? id)
		{
			if(id != null && records.ContainsKey((long)id))
			{
				return records[(long)id];
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
			IGameEntity newInstance = CreateRepositoryType();
			newInstance.SetValue<long>(Field.Id, id);
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