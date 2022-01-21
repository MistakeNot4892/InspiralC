using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Field
	{
		internal static DatabaseField ClientId = new DatabaseField(
			"clientId", "",
			typeof(string), false, false, false);
	}
	internal partial class ComponentModule : GameModule
	{
		private List<GameComponent> Clients => Repositories.Components.GetComponents("client");
	}
	internal class ClientBuilder : GameComponentBuilder
	{
		public ClientBuilder()
		{
			ComponentId = "client";
			schemaFields = new List<DatabaseField>()
			{
				Field.Id,
				Field.Parent,
				Field.ComponentId,
				Field.ClientId
			};
		}
		internal override GameComponent MakeComponent()
		{
			return new ClientComponent();
		}
	}
	internal class ClientComponent : GameComponent 
	{
		internal GameClient? client;
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
