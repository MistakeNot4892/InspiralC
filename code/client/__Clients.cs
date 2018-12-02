using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace inspiral
{
	internal static class Clients
	{
		private static List<GameClient> clients = new List<GameClient>();
		internal static GameClient Create(TcpClient client, string id)
		{
			GameClient newClient = new GameClient(client, id);
			clients.Add(newClient);
			return newClient;
		}
		internal static void RemoveClient(GameClient client)
		{
			clients.Remove(client);
		}
		internal static int CountClients()
		{
			return clients.Count;
		}
	}
}