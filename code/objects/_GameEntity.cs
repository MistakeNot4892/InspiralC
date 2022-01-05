using System.Collections.Generic;

namespace inspiral
{
	internal partial class GameEntity
	{
		internal long id;
		internal string name = "object";
		internal GenderObject gender;
		internal List<string> aliases = new List<string>();
		internal GameEntity location;
		internal List<GameEntity> contents;
		internal long flags = 0;
		internal Dictionary<System.Type, GameComponent> components;
		internal GameEntity()
		{
			contents = new List<GameEntity>();
			components = new Dictionary<System.Type, GameComponent>();
			gender = Modules.Gender.GetByTerm(Text.GenderInanimate);
		}
		internal GameEntity FindGameObjectNearby(string token)
		{
			string checkToken = token.ToLower();
			if(checkToken == "me" || checkToken == "self")
			{
				return this;
			}
			if(location != null)
			{
				if(checkToken == "here" || 
					checkToken == "room" || 
					"{location.id}" == checkToken || 
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
		internal GameEntity FindGameObjectInContents(string token)
		{
			return FindGameObjectInContents(token, true);
		}

		internal GameEntity FindGameObjectInContents(string token, bool silent)
		{
			string checkToken = token.ToLower();
			foreach(GameEntity gameObj in contents)
			{
				if($"{gameObj.id}" == checkToken || 
					gameObj.name.ToLower() == checkToken || 
					gameObj.aliases.Contains(checkToken) || 
					$"{gameObj.name}#{gameObj.id}" == checkToken
					)
				{
					return gameObj;
				}
			}
			foreach(GameEntity gameObj in contents)
			{
				if(gameObj.name.ToLower().Contains(checkToken))
				{
					return gameObj;
				}
				foreach(string alias in gameObj.aliases)
				{
					if(alias.ToLower().Contains(checkToken) || $"{alias}#{gameObj.id}" == checkToken)
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
					GameEntity otherRoom = (GameEntity)Game.Objects.GetByID(room.exits[lookingFor]);
					if(otherRoom != null)
					{
						return otherRoom;
					}
				}
			}
			return null;
		}
		internal bool Collectable(GameEntity collecting)
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
		internal string HandleImpact(GameEntity wielder, GameEntity impacting, double force)
		{
			if(impacting.HasComponent<PhysicsComponent>())
			{
				PhysicsComponent phys = (PhysicsComponent)impacting.GetComponent<PhysicsComponent>();
				double strikePenetration = phys.GetImpactPenetration(force, 1.0);
				if(strikePenetration > 0)
				{
					if(phys.edged)
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
