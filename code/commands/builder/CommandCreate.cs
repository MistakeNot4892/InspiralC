namespace inspiral
{
	internal partial class CommandModule : GameModule 
	{
		internal void CmdCreate(GameObject invoker, CommandData cmd)
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