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
			"exits", "", 
			typeof(string), true, false);
	}

	internal class RoomBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(RoomComponent);
			schemaFields = new List<DatabaseField>() { Field.Exits };
			base.Initialize();
		}
	}

	internal class RoomComponent : GameComponent
	{
		internal Dictionary<string, long> exits = new Dictionary<string, long>();
		internal string GetExitString()
		{
			if(exits.Count <= 0)
			{
				return "You cannot see any exits.";
			}
			else if(exits.Count == 1)
			{
				KeyValuePair<string, long> exit = exits.ElementAt(0);
				return $"You can see a single exit leading {exit.Key}.";
			}
			else
			{
				return $"You can see exits leading {Text.EnglishList(exits)}.";
			}
		}
	}
}
