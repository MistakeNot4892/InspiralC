namespace inspiral
{
	internal interface IGameEntity
	{
		string GetDatabaseTableName();
		bool SetValue<T>(DatabaseField field, T newValue);
		T? GetValue<T>(DatabaseField field);
		void CopyFromRecord(System.Collections.Generic.Dictionary<DatabaseField, object> record);
		System.Collections.Generic.Dictionary<DatabaseField, object> GetSaveData();
	}
}