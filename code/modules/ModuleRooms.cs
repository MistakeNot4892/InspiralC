namespace inspiral
{
	internal partial class Modules
	{
		internal RoomsModule Rooms = new RoomsModule();
	}
	internal class RoomsModule : GameModule
	{
        internal GameObject? DefaultRoom = null;
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