using System.Collections.Generic;
using System.Data.SQLite;

namespace inspiral
{
	internal static partial class Components
	{
		internal const string Client =    "client";
		internal static List<GameComponent> Clients =>  GetComponents(Client);
	}
	internal class ClientBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Components.Client;
		internal override GameComponent Build()
		{
			return new ClientComponent();
		}
	}
	class ClientComponent : GameComponent 
	{
		internal GameClient client;
		internal void Login(GameClient _client)
		{
			client = _client;
		}
		internal void Logout()
		{
			client?.shell?.RemoveComponent(Components.Client);
			client = null;
		}
		internal override string GetStringSummary() 
		{
			return $"Id: {client?.id ?? "no client"}";
		}
	}
}
