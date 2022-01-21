namespace inspiral
{
	internal static partial class Modules
	{
		internal static RoomsModule Rooms { get { return (RoomsModule)GetModule<RoomsModule>(); } }
	}
	internal class RoomsModule : GameModule
	{
        internal GameObject? DefaultRoom = null;
        internal GameObject GetSpawnRoom()
        {
            if(DefaultRoom == null)
            {
                DefaultRoom = CreateEmpty();
                Game.LogError($"Created spawn room at {DefaultRoom.GetValue<ulong>(Field.Id)}.");
            }
            return DefaultRoom;
        }
        internal GameObject CreateEmpty()
        {
            return Repositories.Objects.CreateFromTemplate("room");
        }
    }
}