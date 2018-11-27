using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace inspiral
{
	class GameClient
	{
		private TcpClient client;
		private NetworkStream stream;
		internal string clientId;
		private List<GameContext> contexts;

		internal GameClient(TcpClient _client, string _clientId)
		{
			client = _client;
			clientId = _clientId;
			stream = client.GetStream();
			contexts = new List<GameContext>();
			Console.WriteLine($"{clientId}: client created.");
			contexts.Add(new ContextGeneral());
		}

		private string Sanitize(string input)
		{

			var builder = new StringBuilder(input.Length + 20);
			foreach (var ch in input)
			{
				switch (ch)
				{
					case '\n':
					case '\r':
						break;
					default:
						builder.Append(ch);
						break;
				}
			}
			return builder.ToString();
		}

		internal void Begin()
		{
			WriteToStream($"Welcome, {clientId}.");
			int i;
			byte[] bytes = new byte[256];
			try
			{
				while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
				{
					ReceiveInput(Sanitize(System.Text.Encoding.ASCII.GetString(bytes, 0, i)));
				}
			} 
			catch (ObjectDisposedException e)
			{
				Console.WriteLine($"{clientId}: disconnected.");
			}
			Program.clients.Remove(this);
			client.Close();
		}

		internal void ReceiveInput(string inputMessage)
		{
			Console.WriteLine("{0}: received: {1}", clientId, inputMessage);
			string cmd = inputMessage.ToLower().Split(" ")[0];
			if(inputMessage.Length > cmd.Length)
			{
				inputMessage = inputMessage.Substring(cmd.Length+1);
			}

			bool invoked = false;
			foreach(GameContext context in contexts)
			{
				if(context.InvokeCommand(this, cmd, inputMessage))
				{
					invoked = true;
					break;
				}
			}
			if(!invoked)
			{
				WriteToStream($"Unknown command '{cmd}'.");
			}
		}

		internal void WriteToStream(string message)
		{
			byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
			stream.Write(msg, 0, msg.Length);
			Console.WriteLine("{0}: sent: {1}", clientId, message);
		}

		internal void Quit()
		{
			WriteToStream("Goodbye!");
			client.Close();
		}
	}
}
