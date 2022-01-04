namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdClient(GameObject invoker, CommandData cmd)
		{
			if(invoker.HasComponent<ClientComponent>())
			{
				ClientComponent client = (ClientComponent)invoker.GetComponent<ClientComponent>();
				invoker.WriteLine(client.client.GetClientSummary());
			}
			invoker.SendPrompt();
		}
	}
}