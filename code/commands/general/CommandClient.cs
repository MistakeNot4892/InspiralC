using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdClient(GameObject invoker, CommandData cmd)
		{
			if(invoker.HasComponent(Text.CompClient))
			{
				ClientComponent client = (ClientComponent)invoker.GetComponent(Text.CompClient);
				invoker.WriteLine(client.client.GetClientSummary());
			}
			invoker.SendPrompt();
		}
	}
}