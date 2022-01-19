using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{
	internal static partial class Field
	{
		internal static DatabaseField Id = new DatabaseField(
			"id", 0,
			typeof(int), true, false);
		internal static DatabaseField Name = new DatabaseField(
			"name", "unkn",
			typeof(string), true, true);
		internal static DatabaseField Gender = new DatabaseField(
			"gender", Text.GenderInanimate,
			typeof(string), true, true);
		internal static DatabaseField Aliases = new DatabaseField(
			"aliases", "", 
			typeof(string), true, true);
		internal static DatabaseField Components = new DatabaseField(
			"components", "{}",
			typeof(string), true, true);
		internal static DatabaseField Flags = new DatabaseField(
			"flags", -1,
			typeof(int), true, true);
		internal static DatabaseField Location = new DatabaseField(
			"location", 0,
			typeof(int), true, false);
	}

	internal partial class GameObject : IGameEntity
	{
		internal List<string> aliases = new List<string>();
		private Dictionary<DatabaseField, object> _fields = new Dictionary<DatabaseField, object>()
		{
			{ Field.Id,         Field.Id.fieldDefault },
			{ Field.Name,       Field.Name.fieldDefault },
			{ Field.Gender,     Field.Gender.fieldDefault },
			{ Field.Aliases,    Field.Aliases.fieldDefault },
			{ Field.Components, Field.Components.fieldDefault },
			{ Field.Flags,      Field.Flags.fieldDefault },
			{ Field.Location,   Field.Location.fieldDefault },
		};
		public Dictionary<DatabaseField, object> Fields
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
		public GameObject? Location
		{
			get
			{ 
				var loc = Program.Game.Repos.Objects.GetById(GetValue<ulong>(Field.Location));
				if(loc != null)
				{
					return (GameObject)loc;
				}
				return null; 
			}
			set
			{ 
				if(value != null)
				{
					SetValue<int>(Field.Location, (int)value.GetValue<ulong>(Field.Id));
				}
			}
		}
		private Dictionary<System.Type, GameComponent> _components = new Dictionary<System.Type, GameComponent>();
		public Dictionary<System.Type, GameComponent> Components
		{
			get { return _components; }
			set { _components = value; }
		}
		public bool SetValue<T>(System.Type componentType, DatabaseField field, T newValue)
		{
			GameComponent? comp = GetComponent(componentType);
			if(comp != null)
			{
				return comp.SetValue<T>(field, newValue);
			}
			return false;
		}
		public bool SetValue<T>(DatabaseField field, T? newValue)
		{
			if(newValue != null && Fields.ContainsKey(field))
			{
				Fields[field] = newValue;
				return true;
			}
			return false;
		}
		public T? GetValue<T>(System.Type componentType, DatabaseField field)
		{
			GameComponent? comp = GetComponent(componentType);
			if(comp != null)
			{
				return comp.GetValue<T>(field);
			}
			return default(T);
		}

		public T? GetValue<T>(DatabaseField field)
		{
			if(Fields.ContainsKey(field))
			{
				return (T)Fields[field];
			}
			return default(T);
		}
		public void CopyFromRecord(Dictionary<DatabaseField, object> record) 
		{
			Fields = record;
			List<string>? aliasStrings = JsonConvert.DeserializeObject<List<string>>((string)Fields[Field.Aliases]);
			if(aliasStrings != null)
			{
				aliases = aliasStrings;
			}
			List<ulong>? componentIds = JsonConvert.DeserializeObject<List<ulong>>((string)Fields[Field.Components]);
			if(componentIds != null)
			{
				foreach(ulong compId in componentIds)
				{
					var compInstance = Program.Game.Repos.Components.GetById(compId);
					if(compInstance != null)
					{
						Components.Add(compInstance.GetType(), (GameComponent)compInstance);
					}
				}
			}
		}
		public Dictionary<DatabaseField, object> GetSaveData()
		{
			Fields[Field.Aliases] = JsonConvert.SerializeObject(aliases);
			List<ulong> componentIds = new List<ulong>();
			foreach(GameComponent comp in Components.Values)
			{
				ulong compId = comp.GetValue<ulong>(Field.Id);
				if(compId > 0)
				{
					componentIds.Add(compId);
				}
			}
			Fields[Field.Components] = JsonConvert.SerializeObject(componentIds);
			return Fields;
		}
		internal GameObject? FindGameObjectNearby(string token)
		{
			return FindGameObjectNearby(this, token);
		}
		internal GameObject? FindGameObjectNearby(GameObject viewer, string token)
		{
			string checkToken = token.ToLower();
			if(Program.Game.Repos.Objects.SelfReferenceTokens.Contains(checkToken))
			{
				return viewer;
			}
			GameObject? location = Location;
			if(location != null)
			{
				if(checkToken == "here" || checkToken == "room" || $"{location.GetValue<ulong>(Field.Id)}" == checkToken)
				{
					return location;
				}

				string? checkingToken = location.GetValue<string>(Field.Name);
				if(checkingToken != null && checkToken == checkingToken.ToLower())
				{
					return location;
				}
				if(location.aliases.Contains(checkToken))
				{
					return location;
				}
				return location.FindGameObjectInContents(checkToken);
			}
			return null;
		}
		internal GameObject? FindGameObjectInContents(string token)
		{
			return FindGameObjectInContents(token, true);
		}

		internal GameObject? FindGameObjectInContents(string token, bool silent)
		{
			string checkToken = token.ToLower();
			foreach(GameObject gameObj in Contents)
			{
				if($"{gameObj.GetValue<ulong>(Field.Id)}" == checkToken || $"{gameObj.GetValue<string>(Field.Name)}#{gameObj.GetValue<ulong>(Field.Id)}" == checkToken)
				{
					return gameObj;
				}

				string? checkingToken = gameObj.GetValue<string>(Field.Name);
				if(checkingToken != null && checkToken == checkingToken.ToLower())
				{
					return gameObj;
				}
				if(gameObj.aliases.Contains(checkToken))
				{
					return gameObj;
				}
			}
			var roomComp = GetComponent<RoomComponent>();
			if(roomComp != null)
			{
				RoomComponent room = (RoomComponent)roomComp;
				string lookingFor = checkToken;
				if(Text.shortExits.ContainsKey(lookingFor))
				{
					lookingFor = Text.shortExits[lookingFor];
				}
				if(room.exits.ContainsKey(lookingFor))
				{
					var otherRoom = Program.Game.Repos.Objects.GetById(room.exits[lookingFor]);
					if(otherRoom != null)
					{
						return (GameObject)otherRoom;
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
			var balComp = GetComponent<BalanceComponent>();
			if(balComp != null)
			{
				BalanceComponent bal = (BalanceComponent)balComp;
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
			var balComp = GetComponent<BalanceComponent>();
			if(balComp != null)
			{
				BalanceComponent bal = (BalanceComponent)balComp;
				bal.KnockBalance(balance, msKnock);
				return true;
			}
			return false;
		}
		internal void SendPrompt()
		{
			var clientComp = GetComponent<ClientComponent>();
			if(clientComp != null)
			{
				ClientComponent client = (ClientComponent)clientComp;
				if(client.client != null)
				{
					client.client.SendPrompt();
				}
			}
		}
		internal void Farewell(string farewell)
		{
			var clientComp = GetComponent<ClientComponent>();
			if(clientComp != null)
			{
				ClientComponent client = (ClientComponent)clientComp;
				if(client.client != null)
				{
					client.client.Farewell(farewell);

				}
			}
		}

		internal void SendPrompt(bool forceClear)
		{
			var clientComp = GetComponent<ClientComponent>();
			if(clientComp != null)
			{
				ClientComponent client = (ClientComponent)clientComp;
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

		internal PlayerAccount? GetAccount()
		{
			var clientComp = GetComponent<ClientComponent>();
			if(clientComp != null)
			{
				ClientComponent client = (ClientComponent)clientComp;
				if(client.client != null)
				{
					return client.client.account;
				}
			}
			return null;
		}
		internal string HandleImpact(GameObject wielder, GameObject impacting, double force)
		{
			var physComp = impacting.GetComponent<PhysicsComponent>();
			if(physComp != null)
			{
				PhysicsComponent phys = (PhysicsComponent)physComp;
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
