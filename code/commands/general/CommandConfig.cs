namespace inspiral
{
	internal class CommandConfig : GameCommand
	{
		internal override void Initialize()
		{
			aliases = new System.Collections.Generic.List<string>() { "config" };
			description = "Configures various options related to gameplay and presentation. WIP.";
			usage = "config [option] <value>";
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			invoker.WriteLine("Config not implemented yet sorry."); 
		}
	}
}