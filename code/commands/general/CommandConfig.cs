namespace inspiral
{
	internal class CommandConfig : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("config");
			Description = "Configures various options related to gameplay and presentation. WIP.";
			Usage = "config [option] <value>";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			invoker.WriteLine("Config not implemented yet sorry."); 
		}
	}
}