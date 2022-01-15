using System.Net.Sockets;
using System.Collections.Generic;

namespace inspiral
{
	internal static class Clients
	{
		internal static List<GameClient> Connected = new List<GameClient>();
		internal static GameClient Create(TcpClient client, string id)
		{
			GameClient newClient = new GameClient(client, id);
			Connected.Add(newClient);
			return newClient;
		}
		internal static void RemoveClient(GameClient client)
		{
			Connected.Remove(client);
		}
		internal static int CountClients()
		{
			return Connected.Count;
		}
	}
}