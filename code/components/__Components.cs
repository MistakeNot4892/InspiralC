using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static class Components
	{
		private static Dictionary<int, List<GameComponent>> allComponents;
		internal static Dictionary<int, string> tableSchemas;
		internal static Dictionary<int, string> loadSchemas;
		internal static Dictionary<int, string> updateSchemas;
		internal static Dictionary<int, string> insertSchemas;
		internal const int Client =  0;
		internal const int Visible = 1;
		internal const int Mobile =  2;
		internal const int Room =    3;
		internal static List<GameComponent> Clients =>  GetComponents(Room);
		internal static List<GameComponent> Visibles => GetComponents(Room);
		internal static List<GameComponent> Mobiles =>  GetComponents(Room);
		internal static List<GameComponent> Rooms =>    GetComponents(Room);
		static Components()
		{
			tableSchemas = new Dictionary<int, string>();
			tableSchemas.Add(Visible,  @"components_visible (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE,
				shortDescription TEXT DEFAULT '',
				roomDescription TEXT DEFAULT '',
				examinedDescription TEXT DEFAULT ''
				)");
			tableSchemas.Add(Mobile, @"components_mobile (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				enterMessage TEXT DEFAULT '', 
				leaveMessage TEXT DEFAULT '', 
				deathMessage TEXT DEFAULT ''
				)");
			tableSchemas.Add(Room, @"components_room (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				exits TEXT DEFAULT ''
				)");
			insertSchemas = new Dictionary<int, string>();
			insertSchemas.Add(Visible, @"INSERT INTO components_visible (
				id, 
				shortDescription, 
				roomDescription, 
				examinedDescription
				) VALUES (
				@p0, 
				@p1, 
				@p2, 
				@p3 
				);");
			insertSchemas.Add(Mobile, @"INSERT INTO components_mobile (
				id,
				enterMessage,
				leaveMessage,
				deathMessage
				) VALUES (
				@p0, 
				@p1, 
				@p2, 
				@p3 
				);");
			insertSchemas.Add(Room, @"INSERT INTO components_room (
				id,
				exits
				) VALUES (
				@p0,
				@p1
				);");
			loadSchemas = new Dictionary<int, string>();
			loadSchemas.Add(Visible,   "components_visible");
			loadSchemas.Add(Mobile,    "components_mobile");
			loadSchemas.Add(Room,      "components_room");
			updateSchemas = new Dictionary<int, string>();
			updateSchemas.Add(Visible, "UPDATE components_visible SET shortDescription = @p1, roomDescription = @p2, examinedDescription = @p3 WHERE id = @p0;");
			updateSchemas.Add(Mobile,  "UPDATE components_mobile SET enterMessage = @p1, leaveMessage = @p2, deathMessage = @p3 WHERE id = @p0");
			updateSchemas.Add(Room,    "UPDATE components_room SET exits = @p1 WHERE id = @p0;");
			allComponents = new Dictionary<int, List<GameComponent>>();
			allComponents.Add(Client,  new List<GameComponent>());
			allComponents.Add(Visible, new List<GameComponent>());
			allComponents.Add(Mobile,  new List<GameComponent>());
			allComponents.Add(Room,    new List<GameComponent>());
		}
		internal static GameComponent MakeComponent(int componentKey)
		{
			GameComponent returning = null;
			switch(componentKey)
			{
				case Client:
					returning = new ClientComponent();
					break;
				case Visible:
					returning = new VisibleComponent();
					break;
				case Mobile:
					returning = new MobileComponent();
					break;
				case Room:
					returning = new RoomComponent();
					break;
				default:
					return null;
			}
			allComponents[componentKey].Add(returning);
			return returning;
		}
		internal static List<GameComponent> GetComponents(int componentKey)
		{
			if(allComponents.ContainsKey(componentKey))
			{
				return allComponents[componentKey];
			}
			return new List<GameComponent>();
		}
	}
}