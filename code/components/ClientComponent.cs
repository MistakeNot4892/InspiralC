using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Text
	{
		internal const string CompClient =    "client";
		internal const string FieldClientId = "clientId";
	}
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Clients => GetComponents(Text.CompClient);
	}
	internal class ClientBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Text.CompClient;
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
			client?.shell?.RemoveComponent(Text.CompClient);
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
