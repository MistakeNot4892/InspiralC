using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Field
	{
		internal static DatabaseField Id = new DatabaseField(
			"id", 0,
			typeof(long), true, false);
	}

	internal partial class GameObject : IGameEntity
	{
		private Dictionary<string, object> _fields = new Dictionary<string, object>();
		public Dictionary<string, object> Fields
		{
			get { return _fields; }
			set { _fields = value; }
		}
		private List<GameObject> _contents = new List<GameObject>();
		public List<GameObject> Contents
		{
			get { return _contents; }
			set { _contents = value; }
		}
		public GameObject Location
		{
			get { return (GameObject)Game.Objects.GetByID(GetValue<long>(Field.Location)); }
			set { SetValue<long>(Field.Location, (long)value.GetValue<long>(Field.Id)); }
		}
		private Dictionary<System.Type, GameComponent> _components = new Dictionary<System.Type, GameComponent>();
		public Dictionary<System.Type, GameComponent> Components
		{
			get { return _components; }
			set { _components = value; }
		}
		public Dictionary<string, object> GetSaveData()
		{
			return Fields;
		}
		public bool SetValue<T>(DatabaseField field, T newValue)
		{
			if(Fields.ContainsKey(field.fieldName))
			{
				Fields[field.fieldName] = newValue;
				return true;
			}
			return false;
		}
		public T GetValue<T>(DatabaseField field)
		{
			if(Fields.ContainsKey(field.fieldName))
			{
				return (T)Fields[field.fieldName];
			}
			return default(T);
		}
		public void CopyFromRecord(Dictionary<string, object> record) 
		{
			Fields = record;
		}
		internal GameObject FindGameObjectNearby(string token)
		{
			return FindGameObjectNearby(this, token);
		}
		internal GameObject FindGameObjectNearby(GameObject viewer, string token)
		{
			string checkToken = token.ToLower();
			if(Game.Objects.SelfReferenceTokens.Contains(checkToken))
			{
				return viewer;
			}
			GameObject location = Location;
			if(location != null)
			{
				if(checkToken == "here" || 
					checkToken == "room" || 
					$"{location.GetValue<long>(Field.Id)}" == checkToken || 
					location.GetValue<string>(Field.Name).ToLower() == checkToken || 
					location.GetValue<List<string>>(Field.Aliases).Contains(checkToken)
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
			foreach(GameObject gameObj in Contents)
			{
				if($"{gameObj.GetValue<long>(Field.Id)}" == checkToken || 
					gameObj.GetValue<string>(Field.Name).ToLower() == checkToken || 
					gameObj.GetValue<List<string>>(Field.Aliases).Contains(checkToken) || 
					$"{gameObj.GetValue<string>(Field.Name)}#{gameObj.GetValue<long>(Field.Id)}" == checkToken
					)
				{
					return gameObj;
				}
			}
			foreach(GameObject gameObj in Contents)
			{
				if(gameObj.GetValue<string>(Field.Name).ToLower().Contains(checkToken))
				{
					return gameObj;
				}
				foreach(string alias in gameObj.GetValue<List<string>>(Field.Aliases))
				{
					if(alias.ToLower().Contains(checkToken) || $"{alias}#{gameObj.GetValue<long>(Field.Id)}" == checkToken)
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
