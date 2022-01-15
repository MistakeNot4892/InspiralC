using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Field
	{
		internal static DatabaseField ClientId = new DatabaseField(
			"clientId", "",
			typeof(string), false, false);
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
			schemaFields = new List<DatabaseField>() { Field.ClientId };
			base.Initialize();
		}
	}
	internal class ClientComponent : GameComponent 
	{
		internal GameClient client;
		internal override void InitializeComponent()
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
	}
}
