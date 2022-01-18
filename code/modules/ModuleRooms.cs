namespace inspiral
{
	internal static partial class Modules
	{
		internal static RoomsModule Rooms = new RoomsModule();
	}
	internal class RoomsModule : GameModule
	{
        internal GameObject? DefaultRoom = null;
		internal override void Initialize() 
		{
			Modules.Rooms = this;
        }
        internal GameObject GetSpawnRoom()
        {
            if(DefaultRoom == null)
            {
                DefaultRoom = CreateEmpty();
            }
            return DefaultRoom;
        }
        internal GameObject CreateEmpty()
        {
            return Game.Repositories.Objects.CreateFromTemplate("room");
        }
    }
}