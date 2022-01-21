namespace inspiral
{
	internal class CommandView : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("view");
			Aliases.Add("vv");
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
				GameObject? viewing = invoker.FindGameObjectNearby(cmd.ObjTarget);
				if(viewing == null)
				{
					try
					{	ulong getId = (ulong)System.Convert.ToUInt64(cmd.ObjTarget);
						var viewObj = Repositories.Objects.GetById(getId);
						if(viewObj != null)
						{
							viewing =(GameObject)viewObj;
						}
					}
					catch(System.Exception e) 
					{
						Game.LogError($"Tried to look up a non-int var in the global db: ({e.ToString()})");
					}
					invoker.WriteLine($"Cannot find '{cmd.ObjTarget}'.");
				}
				else
				{
					int wrap = 80;
					var clientComp = invoker.GetComponent<ClientComponent>();
					if(clientComp != null)
					{
						ClientComponent client = (ClientComponent)clientComp;
						if(client.client != null)
						{
							wrap = client.client.config.wrapwidth;
						}
					}
					string viewSummary = viewing.GetStringSummary(invoker, wrap);
					if(viewSummary != "")
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