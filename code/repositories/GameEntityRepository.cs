using System.Collections.Generic;

namespace inspiral
{
	static class GameEntityRepository
	{
		private static Dictionary<long, GameObject> gameObjects;
		static GameEntityRepository()
		{
			gameObjects = new Dictionary<long, GameObject>();
		}
		public static GameObject GetObject(long objectId)
		{
			if(gameObjects.ContainsKey(objectId))
			{
				return gameObjects[objectId];
			}
			return null;
		}
	}
}