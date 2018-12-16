using System.Collections.Generic;
using System.Data.SQLite;

namespace inspiral
{
	internal static partial class Components
	{
		internal const string Client =    "client";
		internal static List<GameComponent> Clients =>  GetComponents(Client);
	}
	internal static partial class Text
	{
		internal const string FieldClientId = "clientId";
	}
	internal class ClientBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Components.Client;
		internal override List<string> viewableFields { get; set; } = new List<string>() {Text.FieldClientId};
		internal override GameComponent Build()
		{
			return new ClientComponent();
		}
	}
	class ClientComponent : GameComponent 
	{
		internal GameClient client;
		internal ClientComponent()
		{
			isPersistent = false;
		}
		internal void Login(GameClient _client)
		{
			client = _client;
		}
		internal void Logout()
		{
			client?.shell?.RemoveComponent(Components.Client);
			client = null;
		}
		internal override string GetString(string field)
		{
			if(field == Text.FieldClientId)
			{
				if(client != null)
				{
					return $"#{client.id}";
				}
				else
				{
					return "null client";
				}
			}
			return null;
		}
	}
}
