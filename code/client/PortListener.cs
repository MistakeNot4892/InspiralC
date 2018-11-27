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
		private TcpListener server = null;
		private int port;
		private string listenerId;

		internal PortListener(int _port)
		{
			port = _port;
			listenerId = port.ToString();
		}

		internal void Begin()
		{
			try
			{
				server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
				server.Start();

				while (true)
				{
					Console.WriteLine("{0}: waiting for a connection...", listenerId);
					TcpClient client = server.AcceptTcpClient();
					Console.WriteLine("{0}: new connection!", listenerId);
					GameClient joiner = new GameClient(client, $"{listenerId}-{Program.clients.Count+1}");
					Program.clients.Add(joiner);
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