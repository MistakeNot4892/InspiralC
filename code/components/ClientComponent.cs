using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Text
	{
		internal const string FieldClientId = "clientId";
	}
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Clients => GetComponents<ClientComponent>();
	}
	internal class ClientBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(ClientComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Text.FieldClientId, (typeof(string), "''", true, false) }
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
