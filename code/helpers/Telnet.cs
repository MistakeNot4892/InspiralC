using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{

	// For reference: http://cryosphere.net/mud-protocol.html
	// and https://www.gammon.com.au/gmcp

	internal static class Telnet
	{
		internal const byte GMCP =             (byte)201;
		internal const byte SE =               (byte)240;
		internal const byte NOP =              (byte)241;
		internal const byte DataMark =         (byte)242;
		internal const byte Break =            (byte)243;
		internal const byte InterruptProcess = (byte)244;
		internal const byte AbortOutput =      (byte)245;
		internal const byte AYT =              (byte)246;
		internal const byte EC =               (byte)247;
		internal const byte EL =               (byte)248;
		internal const byte GA =               (byte)249;
		internal const byte SB =               (byte)250;
		internal const byte WILL =             (byte)251;
		internal const byte WONT =             (byte)252;
		internal const byte DO =               (byte)253;
		internal const byte DONT =             (byte)254;
		internal const byte IAC =              (byte)255;
		internal static Dictionary<byte, string> bytesToStrings;

		static Telnet()
		{
			bytesToStrings = new Dictionary<byte, string>();
			bytesToStrings.Add(GMCP, "GMCP Action");
			bytesToStrings.Add(SE, "End of Subnegotiation");
			bytesToStrings.Add(NOP, "No Operation");
			bytesToStrings.Add(DataMark, "Data Mark");
			bytesToStrings.Add(Break, "Break");
			bytesToStrings.Add(InterruptProcess, "Interrupt Process");
			bytesToStrings.Add(AbortOutput, "Abort Output");
			bytesToStrings.Add(AYT, "Are You There");
			bytesToStrings.Add(EC, "Erase Character");
			bytesToStrings.Add(EL, "Erase Line");
			bytesToStrings.Add(GA, "Go Ahead");
			bytesToStrings.Add(SB, "Begin Subnegotiation");
			bytesToStrings.Add(WILL, "Will");
			bytesToStrings.Add(WONT, "Won't");
			bytesToStrings.Add(DO, "Do");
			bytesToStrings.Add(DONT, "Don't");
			bytesToStrings.Add(IAC, "Incoming Action");
		}

		internal static void SendGMCPPacket(GameClient recipient, string gmcpPacket)
		{
			// IAC SB GMCP Package[.SubPackages].Message <data> IAC SE 
			byte[] outgoingPacket = new byte[gmcpPacket.Length + 5];
			byte[] bytePacket = System.Text.Encoding.ASCII.GetBytes(gmcpPacket);

			outgoingPacket[0] = IAC;
			outgoingPacket[1] = SB;
			outgoingPacket[2] = GMCP;
			outgoingPacket[bytePacket.Length + 3] = IAC;
			outgoingPacket[bytePacket.Length + 4] = SE;

			for(int i = 0;i < bytePacket.Length; i++)
			{
				outgoingPacket[i+3] = bytePacket[i];
			}

			recipient.WriteToStream(outgoingPacket);
		}
		internal static void HandleGMCPNegotiation(GameClient sender, List<byte> gmcpSequence)
		{
			string someSequence = "";
			foreach(byte b in gmcpSequence)
			{
				someSequence += (char)b; 
			}

			int tokenSplitIndex = someSequence.IndexOf(' ');
			if(tokenSplitIndex == -1)
			{
				string gmcpToken = someSequence.Trim().ToLower();
				switch(gmcpToken)
				{
					case "core.keepalive":
						Console.WriteLine($"Got a keepalive ping from {sender.id}.");
						break;
					default:
						Console.WriteLine($"Got unknown GMCP signal ({gmcpToken}) from {sender.id}.");
						break;
				}
			}
			else
			{
				string gmcpToken =    someSequence.Substring(0,tokenSplitIndex).Trim().ToLower();
				string gmcpContents = someSequence.Substring(gmcpToken.Length).Trim();
				Dictionary<string, string> packetContents = null;

				switch(gmcpToken.ToLower())
				{
					case "core.hello":
						packetContents = JsonConvert.DeserializeObject<Dictionary<string, string>>(gmcpContents);
						break;
					case "core.supports.set":
						packetContents = new Dictionary<string, string>();
						foreach(string token in JsonConvert.DeserializeObject<List<string>>(gmcpContents))
						{
							string[] tokenSplit = token.Split(" ");
							packetContents.Add(tokenSplit[0], tokenSplit[1]);
						}
						break;
					default:
						Console.WriteLine($"Got unknown GMCP packet ({gmcpToken}) from {sender.id}. [{gmcpContents}]");
						break;
				}
				if(packetContents != null)
				{
					Console.WriteLine($"Got GMCP packet ({gmcpToken}) from {sender.id}. Contents:");
					foreach(KeyValuePair<string, string> token in packetContents)
					{
						Console.WriteLine($"- {token.Key} = {token.Value}"); 
					}
				}
			}
		}

		internal static void Parse(GameClient sender, byte[] bytes, int i)
		{

			List<byte> sequence = new List<byte>();
			List<List<byte>> allSequences = new List<List<byte>>();
			List<byte> gmcpSequence = null;

			for(int j = 0;j < i;j++)
			{
				byte b = bytes[j];
				switch(b)
				{

					case NOP:
						continue;

					case IAC:
					case SB:
					case SE:
						if(sequence.Count > 0)
						{
							allSequences.Add(sequence);
							sequence = new List<byte>();
						}
						if(gmcpSequence != null)
						{
							if(gmcpSequence.Count > 0)
							{
								HandleGMCPNegotiation(sender, gmcpSequence);
							}
							gmcpSequence = null;
						}
						break;

					case GMCP:
						if(gmcpSequence != null)
						{
							if(gmcpSequence.Count > 0)
							{
								HandleGMCPNegotiation(sender, gmcpSequence);
							}
						}
						gmcpSequence = new List<byte>();
						break;

					case DataMark:
					case Break:
					case InterruptProcess:
					case AbortOutput:
					case AYT:
					case EC:
					case EL:
					case GA:
					case WILL:
					case WONT:
					case DO:
					case DONT:
						if(gmcpSequence != null)
						{
							if(gmcpSequence.Count > 0)
							{
								HandleGMCPNegotiation(sender, gmcpSequence);
							}
							gmcpSequence = null;
						}
						sequence.Add(b);
						break;

					default:
						if(gmcpSequence != null)
						{
							gmcpSequence.Add(b);
						}
						else
						{
							sequence.Add(b);
						}
						break;
				}
			}
			if(sequence.Count > 0)
			{
				allSequences.Add(sequence);
			}
			if(gmcpSequence != null && gmcpSequence.Count > 0)
			{
				HandleGMCPNegotiation(sender, gmcpSequence);
			}
	
			foreach(List<byte> someSequence in allSequences)
			{
				string humanReadableSequence = "";
				foreach(byte b in someSequence)
				{
					if(bytesToStrings.ContainsKey(b))
					{
						humanReadableSequence += $"{bytesToStrings[b]}";
					}	
					else
					{
						humanReadableSequence += $"{(char)b}";
					}
				}
			}
		}
	}
}