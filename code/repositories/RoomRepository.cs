using System;
using System.Data.SQLite;
using System.Collections.Generic;

namespace inspiral
{
	internal class RoomRepository : GameRepository
	{
		internal GameRoomObject spawnRoom;
		internal RoomRepository()
		{
			repoName = "rooms";
			dbTableName = "rooms";
			dbInsertQuery = $"INSERT INTO {dbTableName} (id) VALUES (@p0);";
			dbTableSchema = @"id INTEGER PRIMARY KEY,
				name TEXT DEFAULT 'room', 
				exits TEXT,
				shortDescription TEXT DEFAULT 'an empty room',
				examinedDescription TEXT DEFAULT 'This is a generic empty room. Incredible.',
				flags INTEGER DEFAULT 0";
		}
		internal override void PostInitialize()
		{
			if(contents.Count <= 0)
			{
				Console.WriteLine("No spawn room found, creating an empty one.");
				CreateNewInstance(true);
			}
			spawnRoom = (GameRoomObject)contents[1];
		}
		internal override Object CreateRepositoryType(long id) {
			GameRoomObject room = new GameRoomObject();
			room.id = id;
			return room;
		}
		internal override void AddCommandParameters(SQLiteCommand command, Object instance)
		{
			GameRoomObject room = (GameRoomObject)instance;
			command.Parameters.AddWithValue("@p0", room.id);
		}
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			GameRoomObject room = (GameRoomObject)CreateRepositoryType((long)reader["id"]);
			room.SetString(Text.FieldShortDesc,    reader["shortDescription"].ToString());
			room.SetString(Text.FieldExaminedDesc, reader["examinedDescription"].ToString());
			contents.Add(room.id, room);
		}
	}
}
