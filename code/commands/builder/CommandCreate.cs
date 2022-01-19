namespace inspiral
{
	internal class CommandCreate : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("create");
			Description = "Creates a new database entry for an object or room.";
			Usage = "create [object]";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			bool msgSent = false;
			if(cmd.ObjTarget != null)
			{
				if(cmd.ObjTarget == "room")
				{
					invoker.WriteLine("Rooms must be created only with the addroom command.");
					msgSent = true;
				}
				else
				{
					GameObject prop = Repositories.Objects.CreateFromTemplate(cmd.ObjTarget);
					if(prop != null && invoker.Location != null)
					{
						prop.Move(invoker.Location);
						invoker.WriteLine($"Created {prop.GetShortDesc()} ({prop.GetValue<string>(Field.Name)}#{prop.GetValue<ulong>(Field.Id)}).");
						msgSent = true;
					}
				}
			}
			if(!msgSent)
			{
				invoker.WriteLine($"You can create the following: {Text.EnglishList(Repositories.Objects.GetTemplateNames())}");
			}
		}
	}
}