namespace inspiral
{
	internal class CommandClient : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "client" };
			description = "Shows your client details.";
			usage = "client";
		}
		internal override void InvokeCommand(GameEntity invoker, CommandData cmd)
		{
			if(invoker.HasComponent<ClientComponent>())
			{
				ClientComponent client = (ClientComponent)invoker.GetComponent<ClientComponent>();
				invoker.WriteLine(client.client.GetClientSummary());
			}
		}
	}
}