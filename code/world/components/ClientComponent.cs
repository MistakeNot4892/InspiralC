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
			if(client != null && client.shell != null)
			{
				if(client.shell.location != null)
				{
					client.shell.location.contents.Remove(client.shell);
					client.shell.location.ShowToContents($"{Text.Capitalize(client.shell.GetString(Components.Visible, Text.FieldShortDesc))} departs to their rest.");
					client.shell.location = null;
				}
				client.shell.RemoveComponent(Components.Client);
			client = null;
			}
		}
	}
}
