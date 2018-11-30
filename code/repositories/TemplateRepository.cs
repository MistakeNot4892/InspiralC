using System;
using System.Linq;
using System.Data.SQLite;
using System.Collections.Generic;

namespace inspiral
{

	internal class GameObjectTemplate
	{
		internal long id;
		internal string name = "object";
		internal List<string> aliases = new List<string>();
		internal string shortDescription = "a generic object";
		internal string roomDescription = "A generic object is here.";
		internal string examinedDescription = "This is a generic object. Fascinating stuff.";
		internal string enterMessage = "A generic object enters from the $DIR.";
		internal string leaveMessage = "A generic object leaves to the $DIR.";
		internal string deathMessage = "A generic object lies here, dead.";
		internal long flags = 0;
		internal string GetString(string field)
		{
			switch(field)
			{
				case Text.FieldShortDesc:
					return shortDescription;
				case Text.FieldRoomDesc:
					return roomDescription;
				case Text.FieldExaminedDesc:
					return examinedDescription;
			}
			return "?";
		}
		internal void SetString(string field, string newString)
		{
			switch(field)
			{
				case Text.FieldShortDesc:
					shortDescription = newString;
					break;
				case Text.FieldRoomDesc:
					roomDescription = newString;
					break;
				case Text.FieldExaminedDesc:
					examinedDescription = newString;
					break;
			}
		}
	}

	internal class TemplateRepository : GameRepository
	{
		internal TemplateRepository()
		{
			repoName = "templates";
			dbTableName = "templates";
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
				flags
				) VALUES 
				( @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9 );";
			dbTableSchema = @"id INTEGER PRIMARY KEY UNIQUE,
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
			GameObjectTemplate template = (GameObjectTemplate)CreateRepositoryType((long)reader["id"]);
			template.name =                reader["name"].ToString();
			template.shortDescription =    reader["shortDescription"].ToString();
			template.roomDescription =     reader["roomDescription"].ToString();
			template.examinedDescription = reader["examinedDescription"].ToString();
			template.enterMessage =        reader["enterMessage"].ToString(); 
			template.leaveMessage =        reader["leaveMessage"].ToString(); 
			template.deathMessage =        reader["deathMessage"].ToString();
			template.flags =               (long)reader["flags"];
			template.aliases = new List<string>();
			string[] aliases = reader["aliases"].ToString().Split("|");
			for(int i = 0;i<aliases.Length;i++)
			{
				template.aliases.Add(aliases[i]);
			}
			contents.Add(template.id, template);
		}
		internal override Object CreateRepositoryType(long id) 
		{
			GameObjectTemplate gameObj = new GameObjectTemplate();
			gameObj.id = id;
			return gameObj;
		}
		internal override void AddCommandParameters(SQLiteCommand command, Object instance) 
		{
			GameObjectTemplate template = (GameObjectTemplate)instance;
			command.Parameters.AddWithValue("@p0", template.id);
			command.Parameters.AddWithValue("@p1", template.name);
			command.Parameters.AddWithValue("@p2", string.Join("|", template.aliases.ToArray()));
			command.Parameters.AddWithValue("@p3", template.shortDescription);
			command.Parameters.AddWithValue("@p4", template.roomDescription);
			command.Parameters.AddWithValue("@p5", template.examinedDescription);
			command.Parameters.AddWithValue("@p6", template.enterMessage); 
			command.Parameters.AddWithValue("@p7", template.leaveMessage); 
			command.Parameters.AddWithValue("@p8", template.deathMessage);
			command.Parameters.AddWithValue("@p9", template.flags);
		}
	}
}