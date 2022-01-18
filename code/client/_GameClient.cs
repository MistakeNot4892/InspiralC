using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

// TODO: https://www.gammon.com.au/mushclient/addingservermxp.htm
// TODO: truecolour
// TODO: SSL to Telnet wrapper?
namespace inspiral
{

	class ClientConfig
	{
		internal int wrapwidth = 80;
	}
	class GameClient
	{
		internal string clientId;
		internal TcpClient? client;
		internal NetworkStream stream;
		internal GameObject shell;
		internal GameContext context;
		internal string? lastPrompt = null;
		internal PlayerAccount? account = null;
		internal bool sentPrompt = false;

		internal List<string> gmcpFlags = new List<string>();
		internal Dictionary<string, string> gmcpValues = new Dictionary<string, string>();

		internal ClientConfig config = new ClientConfig();

		private byte[] inputBuffer = new byte[Telnet.MaxBufferSize];
		private int inputBufferIndex = 0;

		private byte[] outputBuffer = new byte[2048];
		private int outputBufferIndex = 0;
		private static GameObject DummyShell = new GameObject();

		internal GameClient(TcpClient _client, string _id)
		{
			client =   _client;
			stream =   _client.GetStream();
			shell =    DummyShell;
			clientId = _id;
			Game.LogError($"{clientId}: client created.");
			context = Contexts.Login;
			context.OnContextSet(this);
		}
		internal void SetContext(GameContext new_context)
		{
			if(context != new_context)
			{
				context.OnContextUnset(this);
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
			//try
			//{
				// Prod them to check if they support GMCP first.
				SendTelnetCommand(new byte[] {Telnet.IAC, Telnet.WILL, Telnet.GMCP});

				// Now start the actual loop.
				while ((i = stream.Read(socketBuffer, 0, socketBuffer.Length)) != 0)
				{
					for(int j = 0;j<i;j++)
					{
						if((char)socketBuffer[j] == '\n' || socketBuffer[j] == Telnet.SE || inputBufferIndex >= Telnet.MaxBufferSize)
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
			/*
			}
			catch (System.Exception e)
			{
				Game.LogError($"{clientId}: disconnected ({e.Message}).");
			}
			finally
			{
			*/
				Disconnect();
				if(client != null)
				{
					client.Close();
				}
			//}
		}
		internal void Disconnect()
		{
			var getClientComp = shell.GetComponent<ClientComponent>();
			if(getClientComp != null)
			{
				ClientComponent clientComp = (ClientComponent)getClientComp;
				if(clientComp.client == this)
				{
					clientComp.Logout();
					shell.ShowNearby(shell, $"{shell.GetShortDesc()} falls asleep.");
					if(shell.HasComponent<VisibleComponent>())
					{
						shell.SetValue<string>(Field.RoomDesc, "$Short is sound asleep here.");
					}
					shell.RemoveComponent<ClientComponent>();
				}
			}
			shell = DummyShell;
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
			if(context != null && !context.TakeInput(this, cmd, rawCmd, inputMessage.Trim()))
			{
				if(Text.exits.Contains(cmd))
				{
					WriteLine($"There is no exit to the {cmd}.");
					SendPrompt();
				}
				else
				{
					WriteLine($"Unknown command '{rawCmd}'.");
					SendPrompt();
				}
			}
		}
		private string FormatOutgoingString(string message)
		{
			message = Text.Wrap(message, config.wrapwidth);
			message = message.Replace("\n","\r\n");
			return message;
		}
		internal void SendPrompt()
		{
			if(sentPrompt)
			{
				return;
			}
			sentPrompt = true;
			string? p = context.GetPrompt(this);
			if(p != null && p.Length > 0)
			{
				WriteToStream(p);
			}
			WriteToStream(new byte[] { Telnet.IAC, Telnet.GA });
			Flush();
		}
		internal void WriteLine(string message)
		{
			if(message != "")
			{
				sentPrompt = false;
				WriteToStream(FormatOutgoingString($"{message}\n"));
				Flush();
			}
		}
		public void WriteToStream(string message)
		{
			WriteToStream(System.Text.Encoding.ASCII.GetBytes(message));
		}
		public void WriteToStream(byte[] message)
		{
			for(int i = 0;i<message.Length;i++)
			{
				if(outputBufferIndex > Telnet.MaxBufferSize)
				{
					Flush();
				}
				outputBufferIndex++;
				outputBuffer[outputBufferIndex] = message[i];
			}
		}

		public void Flush()
		{
			stream.Write(outputBuffer, 0, outputBufferIndex+1);
			outputBufferIndex = 0;
		}
		internal void Quit()
		{
			if(client != null)
			{
				client.Close();
			}
		}
		internal void Farewell(string farewell)
		{
			if(gmcpFlags.Contains("gmcpEnabled"))
			{
				Telnet.SendGMCPPacket(this, $"Core.Goodbye \"{farewell}\"");
			}
			WriteLine($"{farewell}");
			Quit();
		}
		internal void Keepalive()
		{
			return; // todo when timeout is added
		}

		internal string GetClientEndpointString()
		{
			if(client != null && client.Client != null && client.Client.RemoteEndPoint != null)
			{
				IPEndPoint? clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
				return clientEndPoint.Address.ToString();
			}
			return "unknown endpoint";
		}

		internal string GetClientSummary()
		{

			Dictionary<string, List<string>> reply = new Dictionary<string, List<string>>();

			reply.Add("Client Information", new List<string>());
			reply["Client Information"].Add($"Logged in as: {shell?.GetValue<string>(Field.Name) ?? "null"}");
			reply["Client Information"].Add($"Connecting from: {GetClientEndpointString()}");


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
			if(shell != null)
			{
				return Text.FormatBlock(shell, reply, config.wrapwidth);
			}
			return Text.FormatBlock(DummyShell, reply, config.wrapwidth);
		}
	}
}
