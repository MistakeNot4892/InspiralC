using System;
using System.Collections.Generic;

namespace inspiral
{
	class CommandCreate : GameCommand
	{
		internal override void Initialize() 
		{
			commandString = "create";
			aliases = new List<string>() {"create"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length <= 0)
			{
				invoker.WriteLinePrompted("What do you want to create (template, object, room)?");
			}
			else
			{
				switch(tokens[0].ToLower())
				{
					case "template":
						GameObjectTemplate template = (GameObjectTemplate)Game.Templates.CreateNewInstance(true);
						invoker.WriteLinePrompted($"Created a new blank template (#{template.id}).");
						break;
					case "object":
						if(tokens.Length < 1)
						{
							invoker.WriteLinePrompted("You must specify a valid template number for a new object.");
						}
						else
						{
							long templateId = 0;
							bool noTemplate = false;
							try
							{
								templateId = Convert.ToInt64(tokens[1]);
								if(Game.Templates.Get(templateId) == null)
								{
									noTemplate = true;
								}
							}
							catch(Exception E)
							{
								noTemplate = true;
							}
							if(noTemplate)
							{
								invoker.WriteLinePrompted("You must specify a valid template number for a new object.");
							}
							else
							{
								GameObject gameObject = (GameObject)Game.Objects.CreateNewInstance(false);
								gameObject.templateId = templateId;
								Game.Objects.AddDatabaseEntry(gameObject);
								invoker.WriteLinePrompted($"Created {gameObject.GetString(Text.FieldShortDesc)} (#{gameObject.id}) from template #{gameObject.templateId}.");
							}
						}
						break;
					case "room":
						GameRoomObject room = (GameRoomObject)Game.Rooms.CreateNewInstance(true);
						invoker.WriteLinePrompted($"Created {room.GetString(Text.FieldShortDesc)} (#{room.id}).");
						break;
					default:
						invoker.WriteLinePrompted("What do you want to create (template, object, room)?");
						break;
				}
			}
			return true;
		}
	}
}