using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace inspiral
{
	internal class RoomComponent : GameComponent
	{
		internal Dictionary<string, long> exits;

		internal RoomComponent()
		{
			key = Components.Room;
			exits = new Dictionary<string, long>();
		}
		internal string GetExitString()
		{
			if(exits.Count <= 0)
			{
				return "You cannot see any exits.";
			}
			else if(exits.Count == 1)
			{
				KeyValuePair<string, long> exit = exits.ElementAt(0);
				return $"You can see a single exit leading {exit.Key}.";
			}
			else
			{
				string exitString = "";
				int i = 0;
				foreach(KeyValuePair<string, long> exit in exits)
				{
					i++;
					if(i == 1)
					{
						exitString = $"{exit.Key}";
					}
					else if(i == exits.Count)
					{
						exitString = $"{exitString} and {exit.Key}";
					}
					else
					{
						exitString = $"{exitString}, {exit.Key}";
					}

				}
				return $"You can see exits leading {exitString}.";
			}
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			foreach(string str in reader["exits"].ToString().Split("|"))
			{
				string[] exitData = str.Split(":");
				if(exitData.Length == 2)
				{
					try
					{
						long newExit = Int32.Parse(exitData[1].ToLower());
						exits.Add(exitData[0], newExit);
					}
					catch(Exception e)
					{
						Debug.WriteLine($"Exception during exit conversion: {e.ToString()}");
					}
				}
			}
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			string exitString = "";
			int i = 0;
			foreach(KeyValuePair<string, long> exit in exits)
			{
				i++;
				if(i != 1)
				{
					exitString = $"{exitString}|";
				}
				exitString = $"{exitString}{exit.Key}:{exit.Value}";
			}
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", exitString);
		}
	}
}
