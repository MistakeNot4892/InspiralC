using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal static partial class Field
	{
		internal const string Id = "id";
	}
	internal partial class GameEntity : SharedBaseClass
	{
		private long id;

		internal virtual bool SetValue(string key, string newValue) { return false; }
		internal virtual bool SetValue(string key, long newValue) { return false; }
		internal virtual bool SetValue(string key, bool newValue) { return false; }
		internal virtual string GetString(string key) { return null; }
		internal virtual long GetLong(string key) { return 0; }
		internal virtual bool GetBool(string key) { return false; }
		internal virtual List<string> GetStringList(string key) { return null; }
		internal virtual void Initialize() {}
		internal GameEntity() {}
		internal GameEntity(long _id)
		{
			id = _id;
			Initialize();
		}
		internal virtual void CopyFromRecord(DatabaseRecord record) 
		{
			id = (long)record.fields["id"];
		}
		internal virtual Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = new Dictionary<string, object>();
			saveData.Add("id", id);
			return saveData;
		}
	}
	internal partial class GameObject : GameEntity
	{
		internal string name = "object";
		internal GenderObject gender;
		internal List<string> aliases = new List<string>();
		internal GameObject location;
		internal List<GameObject> contents;
		internal long flags = 0;
		internal Dictionary<System.Type, GameComponent> components;
		internal static List<string> selfReferenceTokens = new List<string>() { "me", "self", "myself" };
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = base.GetSaveData();
			saveData.Add(Field.Name,       name);
			saveData.Add(Field.Gender,     gender.Term);
			saveData.Add(Field.Aliases,    JsonConvert.SerializeObject(aliases));
			saveData.Add(Field.Components, JsonConvert.SerializeObject(components.Keys));
			saveData.Add(Field.Flags,      flags);
			saveData.Add(Field.Location,   (location?.GetLong(Field.Id) ?? 0));
			return saveData;
		}

		internal override void Initialize()
		{
			base.Initialize();
			contents = new List<GameObject>();
			components = new Dictionary<System.Type, GameComponent>();
			gender = Modules.Gender.GetByTerm(Text.GenderInanimate);
		}
		internal GameObject FindGameObjectNearby(string token)
		{
			return FindGameObjectNearby(this, token);
		}
		internal GameObject FindGameObjectNearby(GameObject viewer, string token)
		{
			string checkToken = token.ToLower();
			if(selfReferenceTokens.Contains(checkToken))
			{
				return viewer;
			}
			if(location != null)
			{
				if(checkToken == "here" || 
					checkToken == "room" || 
					"{location.GetLong(Field.Id)}" == checkToken || 
					location.name.ToLower() == checkToken || 
					location.aliases.Contains(checkToken)
					)
				{
					return location;
				}
				return location.FindGameObjectInContents(checkToken);
			}
			return null;
		}
		internal GameObject FindGameObjectInContents(string token)
		{
			return FindGameObjectInContents(token, true);
		}

		internal GameObject FindGameObjectInContents(string token, bool silent)
		{
			string checkToken = token.ToLower();
			foreach(GameObject gameObj in contents)
			{
				if($"{gameObj.GetLong(Field.Id)}" == checkToken || 
					gameObj.name.ToLower() == checkToken || 
					gameObj.aliases.Contains(checkToken) || 
					$"{gameObj.name}#{gameObj.GetLong(Field.Id)}" == checkToken
					)
				{
					return gameObj;
				}
			}
			foreach(GameObject gameObj in contents)
			{
				if(gameObj.name.ToLower().Contains(checkToken))
				{
					return gameObj;
				}
				foreach(string alias in gameObj.aliases)
				{
					if(alias.ToLower().Contains(checkToken) || $"{alias}#{gameObj.GetLong(Field.Id)}" == checkToken)
					{
						return gameObj;
					}
				}
			}
			if(HasComponent<RoomComponent>())
			{
				RoomComponent room = (RoomComponent)GetComponent<RoomComponent>();
				string lookingFor = checkToken;
				if(Text.shortExits.ContainsKey(lookingFor))
				{
					lookingFor = Text.shortExits[lookingFor];
				}
				if(room.exits.ContainsKey(lookingFor))
				{
					GameObject otherRoom = (GameObject)Game.Objects.GetByID(room.exits[lookingFor]);
					if(otherRoom != null)
					{
						return otherRoom;
					}
				}
			}
			return null;
		}
		internal bool Collectable(GameObject collecting)
		{
			return !HasComponent<RoomComponent>() && !HasComponent<MobileComponent>();
		}
		internal bool CanUseBalance(string balance)
		{
			if(HasComponent<BalanceComponent>())
			{
				BalanceComponent bal = (BalanceComponent)GetComponent<BalanceComponent>();
				return bal.OnBalance(balance);
			}
			return false;
		}

		internal bool TryUseBalance(string balance, int msKnock)
		{
			return TryUseBalance(balance, msKnock, false);
		}
		internal bool TryUseBalance(string balance)
		{
			return TryUseBalance(balance, 0, false);
		}
		internal bool TryUseBalance(string balance, int msKnock, bool ignoreOffbal)
		{
			if(HasComponent<BalanceComponent>())
			{
				if(ignoreOffbal || CanUseBalance(balance))
				{
					if(msKnock <= 0)
					{
						return true;
					}
					WriteLine($"Using {msKnock}ms of {balance}");
					return UseBalance(balance, msKnock);
				}
				else
				{
					WriteLine($"You must recover your {balance} before you can act again.");
				}
			}
			WriteLine($"You are not capable of performing actions that require {balance}.");
			return false;
		}
		internal bool UseBalance(string balance, int msKnock)
		{
			if(HasComponent<BalanceComponent>())
			{
				BalanceComponent bal = (BalanceComponent)GetComponent<BalanceComponent>();
				bal.KnockBalance(balance, msKnock);
				return true;
			}
			return false;
		}
		internal void SendPrompt()
		{
			if(HasComponent<ClientComponent>())
			{
				ClientComponent client = (ClientComponent)GetComponent<ClientComponent>();
				client.client.SendPrompt();
			}
		}
		internal void Farewell(string farewell)
		{
			if(HasComponent<ClientComponent>())
			{
				ClientComponent client = (ClientComponent)GetComponent<ClientComponent>();
				if(client.client != null)
				{
					client.client.Farewell(farewell);

				}
			}
		}

		internal void SendPrompt(bool forceClear)
		{
			if(HasComponent<ClientComponent>())
			{
				ClientComponent client = (ClientComponent)GetComponent<ClientComponent>();
				if(client.client != null)
				{
					if(forceClear)
					{
						client.client.lastPrompt = null;
						client.client.sentPrompt = false;
					}
					client.client.SendPrompt();
				}
			}
		}

		internal PlayerAccount GetAccount()
		{
			if(HasComponent<ClientComponent>())
			{
				ClientComponent client = (ClientComponent)GetComponent<ClientComponent>();
				return client.client.account;
			}
			return null;
		}
		internal string HandleImpact(GameObject wielder, GameObject impacting, double force)
		{
			if(impacting.HasComponent<PhysicsComponent>())
			{
				PhysicsComponent phys = (PhysicsComponent)impacting.GetComponent<PhysicsComponent>();
				double strikePenetration = phys.GetImpactPenetration(force, 1.0);
				if(strikePenetration > 0)
				{
					if(phys.edged >= 1)
					{
						return $"leaving a {strikePenetration}cm deep, bleeding wound";
					}
					else
					{
						return $"leaving a {strikePenetration}cm deep bruise";
					}
				}
			}
			return "to no effect";
		}
	}
}
