namespace inspiral
{
	internal class CommandClient : GameCommand
	{
		internal override void Initialize()
		{
			Aliases = new System.Collections.Generic.List<string>() { "client" };
			Description = "Shows your client details.";
			Usage = "client";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(invoker.HasComponent<ClientComponent>())
			{
				ClientComponent client = (ClientComponent)invoker.GetComponent<ClientComponent>();
				invoker.WriteLine(client.client.GetClientSummary());
			}
		}
	}
}