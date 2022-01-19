using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace inspiral
{

	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Rooms => GetComponents<RoomComponent>();
	}

	internal static partial class Field
	{
		internal static DatabaseField Exits = new DatabaseField(
			"exits", "[]", 
			typeof(string), true, false, true);
	}

	internal class RoomBuilder : GameComponentBuilder
	{
		public RoomBuilder()
		{
			ComponentType = typeof(RoomComponent);
			schemaFields = new List<DatabaseField>()
			{
				Field.Id,
				Field.Parent,
				Field.Exits
			};
		}
		internal override GameComponent MakeComponent()
		{
			return new RoomComponent();
		}
	}

	internal class RoomComponent : GameComponent
	{
		internal Dictionary<string, ulong> exits = new Dictionary<string, ulong>();
		internal string GetExitString()
		{
			if(exits.Count <= 0)
			{
				return "You cannot see any exits.";
			}
			else if(exits.Count == 1)
			{
				KeyValuePair<string, ulong> exit = exits.ElementAt(0);
				return $"You can see a single exit leading {exit.Key}.";
			}
			else
			{
				return $"You can see exits leading {Text.EnglishList(exits)}.";
			}
		}
	}
}
