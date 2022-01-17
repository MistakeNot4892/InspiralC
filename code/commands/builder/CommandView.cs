namespace inspiral
{
	internal class CommandView : GameCommand
	{
		internal override void Initialize()
		{
			Aliases = new System.Collections.Generic.List<string>() { "view", "vv" };
			Description = "Views the components and fields of an object.";
			Usage = "view [object name or id]";
		}

		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.ObjTarget == null)
			{
				invoker.WriteLine("What do you wish to view?");
			}
			else
			{
				GameObject viewing = invoker.FindGameObjectNearby(cmd.ObjTarget);
				if(viewing == null)
				{
					try
					{
						viewing = (GameObject)Repos.Objects.GetByID((long)System.Convert.ToInt64(cmd.ObjTarget));
					}
					catch(System.Exception e) 
					{
						Game.LogError($"Tried to look up a non-long var in the global db ({e.Message})");
					}
					invoker.WriteLine($"Cannot find '{cmd.ObjTarget}'.");
				}
				else
				{
					int wrap = 80;
					if(invoker.HasComponent<ClientComponent>())
					{
						ClientComponent client = (ClientComponent)invoker.GetComponent<ClientComponent>();
						wrap = client.client.config.wrapwidth;
					}
					string viewSummary = viewing.GetStringSummary(invoker, wrap);
					if(viewSummary != null)
					{
						invoker.WriteLine(viewSummary);
					}
					else
					{
						invoker.WriteLine("No viewable information.");
					}
				}
			}
		}
	}
}