using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace inspiral
{
	class PortListener
	{
		private TcpListener? server = null;
		private int port;
		private string portId;
		internal PortListener(int _port)
		{
			port = _port;
			portId = port.ToString();
		}
		internal void Begin()
		{
			IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
			try
			{
				System.Console.WriteLine($"{portId}: Connecting to address {ipAddress.ToString()} on port {port}");
				server = new TcpListener(ipAddress, port);
				server.Start();
				while(true)
				{
					System.Console.WriteLine("{0}: waiting for a connection...", portId);
					TcpClient client = server.AcceptTcpClient();
					GameClient joiner = Clients.Create(client, $"{portId}-{Clients.CountClients()+1}");
					System.Console.WriteLine("{0}: new connection from {1}", portId, joiner.GetClientEndpointString());
					Task.Run(() => joiner.Begin());
				}
			}
			catch (SocketException e)
			{
				Game.LogError($"SocketException: {e}");
			}
			finally
			{
				if(server != null)
				{
					server.Stop();
				}
			}
		}
	}
}