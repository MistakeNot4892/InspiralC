using System.Data.SQLite;

namespace inspiral
{
	class ClientComponent : GameComponent 
	{
		internal GameClient client;
		internal ClientComponent()
		{
			key = Components.Client;
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
		internal override string GetStringSummary() 
		{
			return $"Id: {client?.id ?? "no client"}";
		}
	}
}
