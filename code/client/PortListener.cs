using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

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
			IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
			try
			{

				Console.WriteLine($"{id}: Connecting to address {ipAddress.ToString()} on port {port}");
				server = new TcpListener(ipAddress, port);
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
				Debug.WriteLine("SocketException: {0}", e);
			}
			finally
			{
				server.Stop();
			}
		}
	}
}