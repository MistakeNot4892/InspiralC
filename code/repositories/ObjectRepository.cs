using System;
using System.Data.SQLite;
using System.Collections.Generic;

namespace inspiral
{
	internal class ObjectRepository : GameRepository
	{

		internal ObjectRepository()
		{
			repoName = "objects";
			dbTableName = "game_objects";
			dbInsertQuery = $@"INSERT INTO {dbTableName} (
				id, 
				name,
				aliases, 
				shortDescription, 
				roomDescription, 
				examinedDescription,
				enterMessage, 
				leaveMessage, 
				deathMessage, 
				components, 
				flags
			) VALUES (
				@p0,
				@p1,
				@p2,
				@p3,
				@p4,
				@p5,
				@p6,
				@p7,
				@p8,
				@p9,
				@p10
			);";
			dbTableSchema = $@"id INTEGER PRIMARY KEY,
				name TEXT DEFAULT '{Text.DefaultName}', 
				aliases TEXT DEFAULT ' ',
				shortDescription TEXT DEFAULT '{Text.DefaultShortDescription}',
				roomDescription TEXT DEFAULT '{Text.DefaultRoomDescription}',
				examinedDescription TEXT DEFAULT '{Text.DefaultExaminedDescription}',
				enterMessage TEXT DEFAULT '{Text.DefaultEnterMessage}', 
				leaveMessage TEXT DEFAULT '{Text.DefaultLeaveMessage}', 
				deathMessage TEXT DEFAULT '{Text.DefaultDeathMessage}',
				components TEXT DEFAULT ' ',
				flags INTEGER DEFAULT -1";
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			GameObject gameObj = (GameObject)CreateRepositoryType((long)reader["id"]);
			gameObj.name = reader["name"].ToString();
			gameObj.flags = (long)reader["flags"];
			gameObj.aliases = new List<string>();
			string[] aliasArray = reader["aliases"].ToString().Split("|");
			for(int i = 0;i < aliasArray.Length; i++)
			{
				gameObj.aliases.Add(aliasArray[i]);
			}
			string[] componentArray = reader["components"].ToString().Split("|");
			for(int i = 0;i < componentArray.Length; i++)
			{
				try
				{
					int comp = Int32.Parse(componentArray[i]);
					gameObj.AddComponent(comp);
				}
				catch(Exception e)
				{
					Console.WriteLine($"Exception when converting component record to key: {e.ToString()}");
				}
			}
			gameObj.SetString(Components.Visible, Text.FieldShortDesc,    reader["shortDescription"].ToString());
			gameObj.SetString(Components.Visible, Text.FieldRoomDesc,     reader["roomDescription"].ToString());
			gameObj.SetString(Components.Visible, Text.FieldExaminedDesc, reader["examinedDescription"].ToString());
			gameObj.SetString(Components.Mobile,  Text.FieldEnterMessage, reader["enterMessage"].ToString());
			gameObj.SetString(Components.Mobile,  Text.FieldLeaveMessage, reader["leaveMessage"].ToString());
			gameObj.SetString(Components.Mobile,  Text.FieldDeathMessage, reader["deathMessage"].ToString());
			contents.Add(gameObj.id, gameObj);
			Console.WriteLine($"Loaded object #{gameObj.id}, {gameObj.GetString(Components.Visible, Text.FieldShortDesc)}.");

		}
		internal override Object CreateRepositoryType(long id) 
		{
			GameObject gameObj = new GameObject();
			gameObj.id = id;
			return gameObj;
		}
		internal override void AddCommandParameters(SQLiteCommand command, Object instance) 
		{
			GameObject gameObj = (GameObject)instance;
			command.Parameters.AddWithValue("@p0", gameObj.id);
			command.Parameters.AddWithValue("@p1", gameObj.name);
			command.Parameters.AddWithValue("@p2", string.Join("|", gameObj.aliases));
			command.Parameters.AddWithValue("@p3", (gameObj.GetComponent(Components.Visible)?
				.GetStringValue(Text.FieldShortDesc))    ?? Text.DefaultShortDescription);
			command.Parameters.AddWithValue("@p4", (gameObj.GetComponent(Components.Visible)?
				.GetStringValue(Text.FieldRoomDesc))     ?? Text.DefaultRoomDescription);
			command.Parameters.AddWithValue("@p5", (gameObj.GetComponent(Components.Visible)?
				.GetStringValue(Text.FieldExaminedDesc)) ?? Text.DefaultExaminedDescription);
			command.Parameters.AddWithValue("@p6", (gameObj.GetComponent(Components.Mobile)?
				.GetStringValue(Text.FieldEnterMessage)) ?? Text.DefaultEnterMessage);
			command.Parameters.AddWithValue("@p7", (gameObj.GetComponent(Components.Mobile)?
				.GetStringValue(Text.FieldLeaveMessage)) ?? Text.DefaultLeaveMessage);
			command.Parameters.AddWithValue("@p8", (gameObj.GetComponent(Components.Mobile)?
				.GetStringValue(Text.FieldDeathMessage)) ?? Text.DefaultDeathMessage);
			List<string> componentKeys = new List<string>();
			foreach(KeyValuePair<int, GameComponent> comp in gameObj.components)
			{
				componentKeys.Add($"{comp.Key}");
			}
			command.Parameters.AddWithValue("@p9", string.Join("|", componentKeys.ToArray()));
			command.Parameters.AddWithValue("@p10", gameObj.flags);
		}

		public override void DumpToConsole() {
			Console.WriteLine($"Dumping {repoName}.");
			foreach(KeyValuePair<long, Object> obj in contents)
			{
				try
				{
					GameObject gameObj = (GameObject)obj.Value;
					Console.WriteLine($"#{gameObj.id} (#{obj.Key} in db) - {gameObj.name} - V[{gameObj.HasComponent(Components.Visible)}] M[{gameObj.HasComponent(Components.Mobile)}] R[{gameObj.HasComponent(Components.Room)}] C[{gameObj.HasComponent(Components.Client)}]");
				}
				catch(Exception e)
				{
					Console.WriteLine($"Malformed or excepting entry in dump - {e.ToString()}");
				}
			}
			Console.WriteLine($"Done.");
		}
	}
}	