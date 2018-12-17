using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

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
				gender,
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
				@p5,
				@p6
			);";
			dbTableSchema = $@"id INTEGER PRIMARY KEY UNIQUE,
				name TEXT DEFAULT '{Text.DefaultName}',
				gender TEXT DEFAULT '{Gender.Inanimate}', 
				aliases TEXT DEFAULT ' ',
				components TEXT DEFAULT ' ',
				flags INTEGER DEFAULT -1,
				location INTEGER";
			dbUpdateQuery =  "UPDATE game_objects SET name = @p1, gender = @p2, aliases = @p3, components = @p4, flags = @p5, location = @p6 WHERE id = @p0;";
		}
		internal override void HandleSecondarySQLInitialization(SQLiteConnection dbConnection)
		{
			foreach(KeyValuePair<string, GameComponentBuilder> builder in Components.builders)
			{
				if(builder.Value.TableSchema != null)
				{
					using( SQLiteCommand command = new SQLiteCommand($"CREATE TABLE IF NOT EXISTS {builder.Value.TableSchema};", dbConnection) )
					{
						try
						{
							command.ExecuteNonQuery();
						}
						catch(Exception e)
						{
							Debug.WriteLine($"Component SQL exception ({builder.Key}): {e.ToString()} - entire query is [{builder.Value.TableSchema}]");
						}
					}
				}
			}
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader, SQLiteConnection dbConnection) 
		{
			GameObject gameObj = (GameObject)CreateRepositoryType((long)reader["id"]);
			gameObj.name = reader["name"].ToString();
			gameObj.flags = (long)reader["flags"];
			gameObj.gender = Gender.GetByTerm(reader["gender"].ToString());
			gameObj.aliases = JsonConvert.DeserializeObject<List<string>>(reader["aliases"].ToString());

			foreach(string comp in JsonConvert.DeserializeObject<List<string>>(reader["components"].ToString()))
			{
				gameObj.AddComponent(comp);
			}
			if(Game.InitComplete)
			{
				LoadComponentData(gameObj);
			}
			contents.Add(gameObj.id, gameObj);
			postInitLocations.Add(gameObj.id, (long)reader["location"]);
		}

		internal void LoadComponentData(GameObject gameObj)
		{
			foreach(KeyValuePair<string, GameComponent> comp in gameObj.components)
			{
				if(Components.builders[comp.Key].LoadSchema != null)
				{
					using( SQLiteCommand command = new SQLiteCommand(Components.builders[comp.Key].LoadSchema, dbConnection))
					{
						try
						{
							command.Parameters.AddWithValue("@p0", gameObj.id);
							SQLiteDataReader reader = command.ExecuteReader();
							while(reader.Read())
							{
								comp.Value.InstantiateFromRecord(reader);
							}
						}
						catch(Exception e)
						{
							Debug.WriteLine($"SQL exception 4 ({repoName}): {e.ToString()} - entire query is [{Components.builders[comp.Key].LoadSchema}]");
						}
					}
				}
			}
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
			foreach(KeyValuePair<string, GameComponent> comp in gameObj.components)
			{
				if(Components.builders[comp.Key].InsertSchema != null)
				{
					using( SQLiteCommand command = new SQLiteCommand(Components.builders[comp.Key].InsertSchema, dbConnection))
					{
						try
						{
							comp.Value.AddCommandParameters(command);
							command.ExecuteNonQuery();
						}
						catch(Exception e)
						{
							Debug.WriteLine($"SQL exception 5 ({repoName}): {e.ToString()} - entire query is [{Components.builders[comp.Key].InsertSchema}]");
						}
					}
				}
			}
		}
		internal override void AddCommandParameters(SQLiteCommand command, Object instance) 
		{
			GameObject gameObj = (GameObject)instance;
			command.Parameters.AddWithValue("@p0", gameObj.id);
			command.Parameters.AddWithValue("@p1", gameObj.name);
			command.Parameters.AddWithValue("@p2", gameObj.gender.Term);
			command.Parameters.AddWithValue("@p3", JsonConvert.SerializeObject(gameObj.aliases));
			List<string> componentKeys = new List<string>();
			foreach(KeyValuePair<string, GameComponent> comp in gameObj.components)
			{
				if(comp.Value.isPersistent)
				{
					componentKeys.Add(comp.Key);
				}
			}
			command.Parameters.AddWithValue("@p4", JsonConvert.SerializeObject(componentKeys));
			command.Parameters.AddWithValue("@p5", gameObj.flags);
			command.Parameters.AddWithValue("@p6", gameObj.location?.id ?? 0);
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
			foreach(KeyValuePair<long, Object> obj in contents)
			{
				LoadComponentData((GameObject)obj.Value);
			}
		}

		public override void HandleAdditionalObjectSave(Object objInstance, SQLiteConnection dbConnection) 
		{
			GameObject gameObj = (GameObject) objInstance;
			foreach(KeyValuePair<string, GameComponent> comp in gameObj.components)
			{
				if(comp.Value.isPersistent && Components.builders[comp.Key].UpdateSchema != null)
				{
					using(SQLiteCommand command = new SQLiteCommand(Components.builders[comp.Key].UpdateSchema, dbConnection))
					{
						try
						{
							comp.Value.AddCommandParameters(command);
							command.ExecuteNonQuery();
						}
						catch(Exception e)
						{
							Debug.WriteLine($"Component SQL exception 2 ({comp.Key}): {e.ToString()} - enter query is [{Components.builders[comp.Key].UpdateSchema}]");
						}
					}
				}
			}
		}
	}
}	