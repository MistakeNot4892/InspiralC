namespace inspiral
{
	internal class CommandCreate : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "create" };
			description = "Creates a new database entry for an object or room.";
			usage = "create [object]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			bool msgSent = false;
			if(cmd.objTarget != null)
			{
				if(cmd.objTarget == "room")
				{
					invoker.WriteLine("Rooms must be created only with the addroom command.");
					msgSent = true;
				}
				else
				{
					GameObject hat = Modules.Templates.Instantiate(cmd.objTarget);
					if(hat != null)
					{
						hat.Move(invoker.location);
						invoker.WriteLine($"Created {hat.GetShortDesc()} ({hat.name}#{hat.GetLong(Field.Id)}).");
						msgSent = true;
					}
				}
			}
			if(!msgSent)
			{
				invoker.WriteLine($"You can create the following: {Text.EnglishList(Modules.Templates.GetTemplateNames())}");
			}
		}
	}
}