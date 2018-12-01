using System;
using System.Collections.Generic;

namespace inspiral
{
	internal static class Components
	{
		private static Dictionary<int, List<GameComponent>> allComponents;
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