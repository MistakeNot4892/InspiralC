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
			dbTableName = "gameobjects";
			dbInsertQuery = $"INSERT INTO {dbTableName} (id) VALUES (@p0);";
			dbTableSchema = @"id INTEGER PRIMARY KEY,
				name TEXT DEFAULT 'object', 
				aliases TEXT,
				shortDescription TEXT DEFAULT 'a generic object',
				roomDescription TEXT DEFAULT 'A generic object is here.',
				examinedDescription TEXT DEFAULT 'This is a generic object. Fascinating stuff.',
				enterMessage TEXT DEFAULT 'A generic object enters from the $DIR.', 
				leaveMessage TEXT DEFAULT 'A generic object leaves to the $DIR.', 
				deathMessage TEXT DEFAULT 'A generic object lies here, dead.', 
				flags INTEGER DEFAULT 0";
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			GameObject gameObj = (GameObject)CreateRepositoryType((long)reader["id"]);
			contents.Add(gameObj.id, gameObj);
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
		}
	}
}	