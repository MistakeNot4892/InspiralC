using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace inspiral
{
	class PortListener
	{
		internal TcpListener server = null;
		internal int port;
		internal string id;

		internal PortListener(int _port)
		{
			port = _port;
			id = port.ToString();
		}

		internal void Begin()
		{
			try
			{
				server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
				server.Start();

				while (true)
				{
					Console.WriteLine("{0}: waiting for a connection...", id);
					TcpClient client = server.AcceptTcpClient();
					Console.WriteLine("{0}: new connection from {1}", id, ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
					GameClient joiner = Clients.Create(client, $"{id}-{Clients.CountClients()+1}");
					Task.Run(() => joiner.Begin());
				}
			}
			catch (SocketException e)
			{
				Console.WriteLine("SocketException: {0}", e);
			}
			finally
			{
				server.Stop();
			}
		}
	}
}