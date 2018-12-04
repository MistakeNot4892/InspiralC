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
			Game.Objects.QueueForUpdate(parent);
		}
		internal virtual void Removed(GameObject takenFrom)
		{
			if(takenFrom == parent)
			{
				parent = null;
			}
			Game.Objects.QueueForUpdate(takenFrom);
		}
		internal virtual bool SetValue(int key, string newValue) { return false; }
		internal virtual bool SetValue(int key, long newValue) { return false; }
		internal virtual string GetString(int key) { return null; }
		internal virtual long GetLong(int key) { return 0; }
		internal virtual void InstantiateFromRecord(SQLiteDataReader reader) {}
		internal virtual void AddCommandParameters(SQLiteCommand command) {}
		internal virtual string GetStringSummary() { return "No values."; }
	}
}
