namespace inspiral
{
	internal class CommandCreate : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "create" };
			description = "Creates a new database entry for an object or room.";
			usage = "create [object or room]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.objTarget != null)
			{
				GameObject hat = Modules.Templates.Instantiate(cmd.objTarget);
				if(hat != null)
				{
					hat.Move(invoker.location);
					invoker.WriteLine($"Created {hat.GetShort()} ({hat.name}#{hat.id}).");
					invoker.SendPrompt();
					return;
				}
			}
			invoker.WriteLine($"You can create the following: {Text.EnglishList(Modules.Templates.GetTemplateNames())}");
			invoker.SendPrompt();
		}
	}
}