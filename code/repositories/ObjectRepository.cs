using System;
using System.Data.SQLite;
using System.Collections.Generic;

namespace inspiral
{
	internal class ObjectRepository : GameRepository
	{
		private Dictionary<long, long> postInitLocations;
		internal ObjectRepository()
		{
			postInitLocations = new Dictionary<long, long>();
			repoName = "objects";
			dbTableName = "game_objects";
			dbInsertQuery = $@"INSERT INTO {dbTableName} (
				id, 
				name,
				aliases, 
				components, 
				flags,
				location
			) VALUES (
				@p0,
				@p1,
				@p2,
				@p3,
				@p4,
				@p5
			);";
			dbTableSchema = $@"id INTEGER PRIMARY KEY UNIQUE,
				name TEXT DEFAULT '{Text.DefaultName}', 
				aliases TEXT DEFAULT ' ',
				components TEXT DEFAULT ' ',
				flags INTEGER DEFAULT -1,
				location INTEGER";
			dbUpdateQuery =  "UPDATE game_objects SET name = @p1, aliases = @p2, components = @p3, flags = @p4, location = @p5 WHERE id = @p0;";
		}

		internal override void HandleSecondarySQLInitialization(SQLiteConnection dbConnection)
		{
			foreach(KeyValuePair<int, string> schema in Components.tableSchemas)
			{
				using( SQLiteCommand command = new SQLiteCommand($"CREATE TABLE IF NOT EXISTS {schema.Value};", dbConnection) )
				{
					try
					{
						command.ExecuteNonQuery();
					}
					catch(Exception e)
					{
						Console.WriteLine($"Component SQL exception ({schema.Key}): {e.ToString()} - entire query is [CREATE TABLE IF NOT EXISTS {schema.Value};]");
					}
				}
			}
		}

		internal override void InstantiateFromRecord(SQLiteDataReader reader, SQLiteConnection dbConnection) 
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
					if(Components.loadSchemas.ContainsKey(comp))
					{
						GameComponent component = gameObj.GetComponent(comp);
						using( SQLiteCommand command = new SQLiteCommand($"SELECT * FROM {Components.loadSchemas[comp]} WHERE id = @p0;", dbConnection))
						{
							try
							{
								command.Parameters.AddWithValue("@p0", gameObj.id);
								SQLiteDataReader secondReader = command.ExecuteReader();
								while(secondReader.Read())
								{
									component.InstantiateFromRecord(secondReader);
								}
							}
							catch(Exception e)
							{
								Console.WriteLine($"SQL exception 4 ({repoName}): {e.ToString()} - entire query is [SELECT * FROM {Components.loadSchemas[comp]} WHERE id = @p0;]");
							}
						}
					}
				}
				catch(Exception e)
				{
					Console.WriteLine($"Exception when converting component record to key: {e.ToString()}");
				}
			}
			contents.Add(gameObj.id, gameObj);
			postInitLocations.Add(gameObj.id, (long)reader["location"]);
		}
		internal override Object CreateRepositoryType(long id) 
		{
			GameObject gameObj = new GameObject();
			gameObj.id = id;
			return gameObj;
		}

		public override void HandleAdditionalSQLInsertion(Object newInstance, SQLiteConnection dbConnection) 
		{
			GameObject gameObj = (GameObject)newInstance;
			foreach(KeyValuePair<int, GameComponent> comp in gameObj.components)
			{
				if(!Components.insertSchemas.ContainsKey(comp.Key))
				{
					continue;
				}
				using( SQLiteCommand command = new SQLiteCommand(Components.insertSchemas[comp.Key], dbConnection))
				{
					try
					{
						comp.Value.AddCommandParameters(command);
						command.ExecuteNonQuery();
					}
					catch(Exception e)
					{
						Console.WriteLine($"SQL exception 5 ({repoName}): {e.ToString()} - entire query is [{Components.insertSchemas[comp.Key]}]");
					}
				}
			}
		}
		internal override void AddCommandParameters(SQLiteCommand command, Object instance) 
		{
			GameObject gameObj = (GameObject)instance;
			command.Parameters.AddWithValue("@p0", gameObj.id);
			command.Parameters.AddWithValue("@p1", gameObj.name);
			command.Parameters.AddWithValue("@p2", string.Join("|", gameObj.aliases));
			List<string> componentKeys = new List<string>();
			foreach(KeyValuePair<int, GameComponent> comp in gameObj.components)
			{
				componentKeys.Add($"{comp.Key}");
			}
			command.Parameters.AddWithValue("@p3", string.Join("|", componentKeys.ToArray()));
			command.Parameters.AddWithValue("@p4", gameObj.flags);
			command.Parameters.AddWithValue("@p5", gameObj.location?.id ?? 0);
		}
		internal long CreateNewEmptyRoom()
		{
			GameObject tmpRoom = (GameObject)Game.Objects.CreateNewInstance(false);
			tmpRoom.AddComponent(Components.Room);
			tmpRoom.AddComponent(Components.Visible);
			tmpRoom.SetString(Components.Visible, Text.FieldShortDesc, Text.DefaultRoomShort);
			tmpRoom.SetString(Components.Visible, Text.FieldExaminedDesc, Text.DefaultRoomLong);
			Game.Objects.AddDatabaseEntry(tmpRoom);
			return tmpRoom.id;
		}
		internal override void PostInitialize() 
		{
			foreach(KeyValuePair<long, long> loc in postInitLocations)
			{
				if(loc.Value > 0)
				{
					GameObject obj =   (GameObject)Get(loc.Key);
					GameObject other = (GameObject)Get(loc.Value);
					if(obj != null && other != null)
					{
						obj.Move(other);
					}
				}
			}
		}
		public override void HandleAdditionalObjectSave(Object objInstance, SQLiteConnection dbConnection) 
		{
			GameObject gameObj = (GameObject) objInstance;
			Console.WriteLine($"Saving modules for {gameObj.id}.");
			foreach(KeyValuePair<int, GameComponent> comp in gameObj.components)
			{
				if(!Components.updateSchemas.ContainsKey(comp.Key))
				{
					continue;
				}
				using(SQLiteCommand command = new SQLiteCommand(Components.updateSchemas[comp.Key], dbConnection))
				{
					try
					{
						comp.Value.AddCommandParameters(command);
						command.ExecuteNonQuery();
					}
					catch(Exception e)
					{
						Console.WriteLine($"Component SQL exception 2 ({comp.Key}): {e.ToString()} - enter query is [{Components.updateSchemas[comp.Key]}]");
					}
				}
			}
		}
	}
}	