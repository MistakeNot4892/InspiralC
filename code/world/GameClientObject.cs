using System;
using System.Collections.Generic;

namespace inspiral
{
	class GameClientObject : GameObject
	{
		private GameClient client;

		internal override bool HasClient()
		{
			return (client != null);
		}

		internal override GameClient GetClient()
		{
			return client;
		}

		internal override void Login(GameClient _client)
		{
			client = _client;
		}

		internal override void Logout()
		{
			client = null;
			if(location != null)
			{
				location.contents.Remove(this);
				location.ShowToContents($"{Text.Capitalize(GetString(Text.FieldShortDesc))} departs to their rest.");
				location = null;
			}
		}
	}
}
