using System;
using System.Data.SQLite;

namespace inspiral
{
	internal class GameComponent
	{
		internal int key;
		internal GameObject parent;
		internal virtual void Added(GameObject addedTo)
		{
			parent = addedTo;
		}
		internal virtual void Removed(GameObject takenFrom)
		{
			if(takenFrom == parent)
			{
				parent = null;
			}
		}
		internal virtual void SetValue(int key, string newValue) {}
		internal virtual void SetValue(int key, long newValue) {}
		internal virtual string GetStringValue(int key) { return null; }
		internal virtual long GetLongValue(int key) { return 0; }
		internal virtual void InstantiateFromRecord(SQLiteDataReader reader) {}
		internal virtual void AddCommandParameters(SQLiteCommand command) {}
	}
}
