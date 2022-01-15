using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Field
	{
		internal const string ClientId = "clientId";
	}
	internal partial class ComponentModule : GameModule
	{
		private List<GameComponent> Clients => GetComponents<ClientComponent>();
	}
	internal class ClientBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(ClientComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Field.ClientId, (typeof(string), "''", true, false) }
			};
			base.Initialize();
		}
	}
	class ClientComponent : GameComponent 
	{
		internal GameClient client;
		internal override void Initialize()
		{
			isPersistent = false;
		}
		internal void Login(GameClient _client)
		{
			client = _client;
		}
		internal void Logout()
		{
			if(client != null)
			{
				if(client.shell != null)
				{
					client.shell.RemoveComponent<ClientComponent>();
				}
				client = null;
			}
		}
		internal override string GetString(string field)
		{
			if(field == Field.ClientId)
			{
				if(client != null)
				{
					return $"#{client.clientId}";
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
