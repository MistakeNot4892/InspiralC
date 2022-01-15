namespace inspiral
{
	internal static partial class Modules
	{
		internal static RoomsModule Rooms;
	}
	internal class RoomsModule : GameModule
	{
		internal override void Initialize() 
		{
			Modules.Rooms = this;
        }
        internal GameObject GetSpawnRoom()
        {
            return null;
        }
        internal GameObject CreateEmpty()
        {
            return null;
        }
    }
}