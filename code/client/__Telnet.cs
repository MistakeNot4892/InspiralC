using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;

namespace inspiral
{

	// For reference: http://cryosphere.net/mud-protocol.html
	// and https://www.gammon.com.au/gmcp

	internal static class Telnet
	{
		internal const byte GMCP = (byte)201;
		internal const byte SE =   (byte)240;
		internal const byte NOP =  (byte)241;
		internal const byte DM =   (byte)242;
		internal const byte B =    (byte)243;
		internal const byte IP =   (byte)244;
		internal const byte AO =   (byte)245;
		internal const byte AYT =  (byte)246;
		internal const byte EC =   (byte)247;
		internal const byte EL =   (byte)248;
		internal const byte GA =   (byte)249;
		internal const byte SB =   (byte)250;
		internal const byte WILL = (byte)251;
		internal const byte WONT = (byte)252;
		internal const byte DO =   (byte)253;
		internal const byte DONT = (byte)254;
		internal const byte IAC =  (byte)255;
		internal const int MaxBufferSize = 1024;
		internal static Dictionary<byte, string> bytesToStrings;

		static Telnet()
		{
			bytesToStrings = new Dictionary<byte, string>();
			bytesToStrings.Add(GMCP, "GMCP Action");
			bytesToStrings.Add(SE,   "End of Subnegotiation");
			bytesToStrings.Add(NOP,  "No Operation");
			bytesToStrings.Add(DM,   "Data Mark");
			bytesToStrings.Add(B,    "Break");
			bytesToStrings.Add(IP,   "Interrupt Process");
			bytesToStrings.Add(AO,   "Abort Output");
			bytesToStrings.Add(AYT,  "Are You There");
			bytesToStrings.Add(EC,   "Erase Character");
			bytesToStrings.Add(EL,   "Erase Line");
			bytesToStrings.Add(GA,   "Go Ahead");
			bytesToStrings.Add(SB,   "Begin Subnegotiation");
			bytesToStrings.Add(WILL, "Will");
			bytesToStrings.Add(WONT, "Won't");
			bytesToStrings.Add(DO,   "Do");
			bytesToStrings.Add(DONT, "Don't");
			bytesToStrings.Add(IAC,  "Incoming Action");
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
						sender.Keepalive();
						break;
					default:
						if(!sender.gmcpFlags.Contains(gmcpToken))
						{
							sender.gmcpFlags.Add(gmcpToken);
						}
						break;
				}
			}
			else
			{
				string gmcpToken =    someSequence.Substring(0,tokenSplitIndex).Trim().ToLower();
				string gmcpContents = someSequence.Substring(gmcpToken.Length).Trim();
				switch(gmcpToken.ToLower())
				{
					case "core.hello":
						if(!sender.gmcpFlags.Contains("gmcpEnabled"))
						{
							sender.gmcpFlags.Add("gmcpEnabled");
						}
						foreach(KeyValuePair<string, string> token in JsonConvert.DeserializeObject<Dictionary<string, string>>(gmcpContents))
						{
							string tokenKey = token.Key.ToLower();
							if(sender.gmcpValues.ContainsKey(tokenKey))
							{
								sender.gmcpValues.Remove(tokenKey);
							}
							sender.gmcpValues.Add(tokenKey, token.Value);
						}
						break;
					case "core.supports.set":
						foreach(string token in JsonConvert.DeserializeObject<List<string>>(gmcpContents))
						{
							string[] tokenSplit = token.Split(" ");
							string tokenKey = tokenSplit[0].ToLower();
							if(tokenSplit[1] == "1")
							{
								if(!sender.gmcpFlags.Contains(tokenKey))
								{
									sender.gmcpFlags.Add(tokenKey);
								}
							}
							else if(tokenSplit[1] == "0")
							{
								if(sender.gmcpFlags.Contains(tokenKey))
								{
									sender.gmcpFlags.Remove(tokenKey);
								}
							}
							else
							{
								if(sender.gmcpValues.ContainsKey(tokenKey))
								{
									sender.gmcpValues.Remove(tokenKey);
								}
								sender.gmcpValues.Add(tokenKey, tokenSplit[1]);
							}
						}
						break;
					default:
						Debug.WriteLine($"Got unknown GMCP packet ({gmcpToken}) from {sender.id}. [{gmcpContents}]");
						break;
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
					case DM:
					case B:
					case IP:
					case AO:
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