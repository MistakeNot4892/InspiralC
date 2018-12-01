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
		internal string id;
		internal GameObject shell;
		internal TcpClient client;
		internal NetworkStream stream;
		internal GameContext context;
		internal PlayerAccount currentAccount = null;
		private enum Options {}

		internal GameClient(TcpClient _client, string _id)
		{
			client =   _client;
			id = _id;
			stream =   _client.GetStream();
			Console.WriteLine($"{id}: client created.");
			SetContext(new ContextLogin());
		}

		internal void SetContext(GameContext new_context)
		{
			if(context != new_context)
			{
				if(context != null)
				{
					context.OnContextUnset(this);
				}
				context = new_context;
				context.OnContextSet(this);
			}
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

		internal void SendTelnetCommand(byte[] sequence)
		{
			WriteToStream(sequence);
		}
		internal void Begin()
		{
			int i;
			byte[] bytes = new byte[256];
			try
			{
				// Prod them to check if they support GMCP first.
				SendTelnetCommand(new byte[] {Telnet.IAC, Telnet.WILL, Telnet.GMCP});

				// Now start the actual loop.
				while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
				{
					if(bytes[0] == Telnet.IAC)
					{
						Telnet.Parse(this, bytes, i);
					}
					else
					{
						string inputReceived = SanitizeInput(System.Text.Encoding.ASCII.GetString(bytes, 0, i));
						if(inputReceived.Length > 0)
						{
							ReceiveInput(inputReceived);
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"{id}: disconnected ({e.ToString()}).");
			}
			Disconnect();
			if(client != null)
			{
				client.Close();
			}
		}
		internal void Disconnect()
		{
			if(shell != null && shell.HasComponent(Components.Client))
			{
				ClientComponent clientComp = (ClientComponent)shell.GetComponent(Components.Client);
				if(clientComp.client == this)
				{
					clientComp.Logout();
					shell.RemoveComponent(Components.Client);
				}
			}
			shell = null;
			Clients.RemoveClient(this);
		}
		internal void ReceiveInput(string inputMessage)
		{
			string rawCmd = inputMessage.Split(" ")[0];
			string cmd = rawCmd.ToLower();
			if(inputMessage.Length > cmd.Length)
			{
				inputMessage = inputMessage.Substring(cmd.Length+1);
			}
			else
			{
				inputMessage = "";
			}
			if(!context.TakeInput(this, cmd, rawCmd, inputMessage))
			{
				if(Text.exits.Contains(cmd))
				{
					WriteLinePrompted($"There is no exit to the {cmd}.");
				}
				else
				{
					WriteLinePrompted($"Unknown command '{rawCmd}'.");
				}
			}
		}

		internal void WriteLinePrompted(string message)
		{
			if(message != "")
			{
				message = $"{message}{context.GetPrompt(this).ToString()}";
				byte[] bMessage = System.Text.Encoding.ASCII.GetBytes(message);
				byte[] outgoing = new byte[bMessage.Length+2];
				int i = 0;
				while(i < bMessage.Length)
				{
					outgoing[i] = bMessage[i];
					i++;
				}
				outgoing[i] = Telnet.IAC;
				outgoing[i+1] = Telnet.GA;
				WriteToStream(outgoing);
			}
		}
		internal void WriteLine(string message)
		{
			if(message != "")
			{
				Console.WriteLine($"sending: {message}");
				WriteToStream($"{message}\n");
			}
		}
		public void WriteToStream(string message)
		{
			WriteToStream(System.Text.Encoding.ASCII.GetBytes(message));
		}
		public void WriteToStream(byte[] message)
		{
			stream.Write(message, 0, message.Length);
		}
		internal void Quit()
		{
			client.Close();
		}
	}
}
