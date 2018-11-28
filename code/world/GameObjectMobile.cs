using System;
using System.Collections.Generic;

namespace inspiral
{
	class GameClientObject : GameObject
	{
		private GameClient client;

		public override bool HasClient()
		{
			return (client != null);
		}

		public override GameClient GetClient()
		{
			return client;
		}

		public override void Login(GameClient _client)
		{
			client = _client;
		}

		public override void Logout()
		{
			client = null;
			if(location != null)
			{
				location.contents.Remove(this);
				location.ShowToContents($"{GameText.Capitalize(GetString("short_description"))} departs to their rest.");
				location = null;
			}
		}
	}
}
