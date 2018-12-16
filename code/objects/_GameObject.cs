using System;
using System.Collections.Generic;

namespace inspiral
{
	internal partial class GameObject
	{
		internal long id;
		internal string name = "object";
		internal GenderObject gender;
		internal List<string> aliases = new List<string>();
		internal GameObject location;
		internal List<GameObject> contents;
		internal long flags = 0;
		internal Dictionary<string, GameComponent> components;
		internal GameObject()
		{
			contents = new List<GameObject>();
			components = new Dictionary<string, GameComponent>();
			gender = Gender.GetByTerm(Gender.Inanimate);
		}
		internal GameObject FindGameObjectNearby(string token)
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
		internal GameObject FindGameObjectInContents(string token)
		{
			return FindGameObjectInContents(token, true);
		}

		internal GameObject FindGameObjectInContents(string token, bool silent)
		{
			string checkToken = token.ToLower();
			foreach(GameObject gameObj in contents)
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
			foreach(GameObject gameObj in contents)
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
			if(HasComponent(Components.Room))
			{
				RoomComponent room = (RoomComponent)GetComponent(Components.Room);
				string lookingFor = checkToken;
				if(Text.shortExits.ContainsKey(lookingFor))
				{
					lookingFor = Text.shortExits[lookingFor];
				}
				if(room.exits.ContainsKey(lookingFor))
				{
					GameObject otherRoom = (GameObject)Game.Objects.Get(room.exits[lookingFor]);
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
			return !HasComponent(Components.Room) && !HasComponent(Components.Mobile);
		}

		internal bool TryUseBalance(string balance, int msKnock)
		{
			return TryUseBalance(balance, msKnock, false);
		}
		internal bool CanUseBalance(string balance)
		{
			if(HasComponent(Components.Balance))
			{
				BalanceComponent bal = (BalanceComponent)GetComponent(Components.Balance);
				return bal.OnBalance(balance);
			}
			return false;
		}
		internal bool TryUseBalance(string balance, int msKnock, bool ignoreOffbal)
		{
			if(HasComponent(Components.Balance))
			{
				if(ignoreOffbal || CanUseBalance(balance))
				{
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
			if(HasComponent(Components.Balance))
			{
				BalanceComponent bal = (BalanceComponent)GetComponent(Components.Balance);
				bal.KnockBalance(balance, msKnock);
				return true;
			}
			return false;
		}
	}
}
