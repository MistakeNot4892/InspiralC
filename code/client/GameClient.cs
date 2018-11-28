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
		internal string clientId;
		internal GameClientObject currentGameObject;
		private TcpClient client;
		private NetworkStream stream;
		private GameContext context;

		internal GameClient(TcpClient _client, string _clientId)
		{
			client =   _client;
			clientId = _clientId;
			stream =   _client.GetStream();

			currentGameObject = new GameClientObject();
			currentGameObject.SetString("short_description",    "UNNAMED");
			currentGameObject.SetString("room_description",     "UNNAMED is here.");
			currentGameObject.SetString("examined_description", "This is some kind of generic mob.");
			currentGameObject.Login(this);

			Console.WriteLine($"{clientId}: client created.");
			SetContext(new ContextLogin());
		}

		public void SetContext(GameContext new_context)
		{
			context = new_context;
			context.OnContextSet(this);
		}
		private string SanitizeInput(string input)
		{
			var builder = new StringBuilder(input.Length);
			foreach (var ch in input.Trim())
			{
				switch (ch)
				{
					case '\\':
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
			int i;
			byte[] bytes = new byte[256];
			try
			{
				while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
				{
					ReceiveInput(SanitizeInput(System.Text.Encoding.ASCII.GetString(bytes, 0, i)));
				}
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine($"{clientId}: disconnected ({e.ToString()}).");
			}
			Disconnect();
			client.Close();
		}

		internal void Disconnect()
		{
			if(currentGameObject != null && currentGameObject.HasClient() && currentGameObject.GetClient() == this)
			{
				currentGameObject.Logout();
			}
			Program.clients.Remove(this);
		}
		internal void ReceiveInput(string inputMessage)
		{
			Console.WriteLine("{0}: received: {1}", clientId, inputMessage);
			string cmd = inputMessage.ToLower().Split(" ")[0];
			if(inputMessage.Length > cmd.Length)
			{
				inputMessage = inputMessage.Substring(cmd.Length+1);
			}
			if(!context.TakeInput(this, cmd, inputMessage))
			{
				WriteToStream($"Unknown command '{cmd}'.");
			}
		}

		internal void WriteLinePrompted(string message)
		{
			string prompt = context.GetPrompt(this);
			WriteLine($"{message}{prompt}");
		}
		internal void WriteLine(string message)
		{
			if(message != "")
			{
				WriteToStream($"{message}\n");
			}
		}
		private void WriteToStream(string message)
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
