using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

//TODO: https://www.gammon.com.au/mushclient/addingservermxp.htm

namespace inspiral
{

	class ClientConfig
	{
		internal int wrapwidth = 80;
	}
	class GameClient
	{
		internal string id;
		internal GameObject shell;
		internal TcpClient client;
		internal NetworkStream stream;
		internal GameContext context;
		internal PlayerAccount currentAccount = null;
		internal List<string> gmcpFlags = new List<string>();
		internal Dictionary<string, string> gmcpValues = new Dictionary<string, string>();

		internal ClientConfig config = new ClientConfig();

		private byte[] inputBuffer = new byte[Telnet.MaxBufferSize];
		private int inputBufferIndex = 0;

		internal GameClient(TcpClient _client, string _id)
		{
			client =   _client;
			id = _id;
			stream =   _client.GetStream();
			Debug.WriteLine($"{id}: client created.");
			SetContext(Contexts.Login);
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
			byte[] socketBuffer = new byte[Telnet.MaxBufferSize];
			try
			{
				// Prod them to check if they support GMCP first.
				SendTelnetCommand(new byte[] {Telnet.IAC, Telnet.WILL, Telnet.GMCP});

				// Now start the actual loop.
				while ((i = stream.Read(socketBuffer, 0, socketBuffer.Length)) != 0)
				{
					for(int j = 0;j<i;j++)
					{
						if((char)socketBuffer[j] == '\n' || (char)socketBuffer[j] == Telnet.SE || inputBufferIndex >= Telnet.MaxBufferSize)
						{
							if(inputBufferIndex > 0)
							{
								if(inputBuffer[0] == Telnet.IAC)
								{
									Telnet.Parse(this, inputBuffer, inputBufferIndex);
								}
								else
								{
									string sendingInput = SanitizeInput(System.Text.Encoding.ASCII.GetString(inputBuffer, 0, inputBufferIndex));
									if(sendingInput.Length > 0)
									{
										ReceiveInput(sendingInput);
									}
								}
								inputBufferIndex = 0;
							}
						}
						else
						{
							inputBuffer[inputBufferIndex] = socketBuffer[j];
							inputBufferIndex++;
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine($"{id}: disconnected ({e.ToString()}).");
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
			if(!context.TakeInput(this, cmd, rawCmd, inputMessage.Trim()))
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

		private string FormatOutgoingString(string message)
		{
			message = Text.Wrap(message, config.wrapwidth);
			message = message.Replace("\n","\r\n");
			return message;
		}
		internal void WriteLinePrompted(string message)
		{
			if(message != "")
			{
				message = FormatOutgoingString(message);
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
				message = FormatOutgoingString($"{message}\n");
				WriteToStream($"{message}");
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
		internal void Farewell(string farewell)
		{
			if(gmcpFlags.Contains("gmcpEnabled"))
			{
				Telnet.SendGMCPPacket(this, $"Core.Goodbye \"{farewell}\"");
			}
			else
			{
				WriteLine($"{farewell}");
			}
			Quit();
		}

		internal void Keepalive()
		{
			return; // todo when timeout is added
		}
		internal string GetClientSummary()
		{

			Dictionary<string, List<string>> reply = new Dictionary<string, List<string>>();

			reply.Add("Client Information", new List<string>());
			reply["Client Information"].Add($"Logged in as: {shell?.name ?? "null"}");

			if(!gmcpFlags.Contains("gmcpEnabled"))
			{
				reply["Client Information"].Add($"\nGMCP is not enabled.");
			}
			else
			{
				reply.Add("GMCP Flags", new List<string>());
				foreach(string gmcpFlag in gmcpFlags)
				{
					reply["GMCP Flags"].Add($"- {gmcpFlag}");
				}
				reply.Add("GMCP Values", new List<string>());
				foreach(KeyValuePair<string, string> gmcpValue in gmcpValues)
				{
					reply["GMCP Values"].Add($"- {gmcpValue.Key}: {gmcpValue.Value}");
				}
			}
			return Text.FormatBlock(reply, config.wrapwidth);
		}
	}
}
