namespace inspiral
{
	internal class CommandClient : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("client");
			Description = "Shows your client details.";
			Usage = "client";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			var clientComp = invoker.GetComponent<ClientComponent>();
			if(clientComp != null)
			{
				ClientComponent client = (ClientComponent)clientComp;
				if(client.client != null)
				{
					invoker.WriteLine(client.client.GetClientSummary());
				}
			}
		}
	}
}