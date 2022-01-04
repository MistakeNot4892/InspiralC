using System.Diagnostics;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdView(GameObject invoker, CommandData cmd)
		{
			if(cmd.objTarget == null)
			{
				invoker.WriteLine("What do you wish to view?");
			}
			else
			{
				GameObject viewing = invoker.FindGameObjectNearby(cmd.objTarget);
				if(viewing == null)
				{
					try
					{
						viewing = (GameObject)Game.Objects.Get((long)System.Convert.ToInt64(cmd.objTarget));
					}
					catch(System.Exception e) 
					{
						Debug.WriteLine($"Tried to look up a non-long var in the global db ({e.Message})");
					}
					invoker.WriteLine($"Cannot find '{cmd.objTarget}'.");
				}
				else
				{
					int wrap = 80;
					if(invoker.HasComponent<ClientComponent>())
					{
						ClientComponent client = (ClientComponent)invoker.GetComponent<ClientComponent>();
						wrap = client.client.config.wrapwidth;
					}
					invoker.WriteLine(viewing.GetStringSummary(wrap));
				}
			}
			invoker.SendPrompt();
		}
	}
}