This is a WIP, it will be added to as I remember/find time.

# Development Guides

## Adding commands

Define the strings relating to your command in a `command_foo.json` file and place it in `data\definitions\commands`. An example of the required definition is:
````
{
	"command" : "foo",
	"description" : "Does some foo related task.",
	"aliases" : 
	[
		"foo",
		"otherfoo",
		"dennis"
	],
	"usage" : "foo [whatever args are expected]",
	"method" : "CmdFoo"
}
````

The actual logic of your command will be a static method on the `Command` class, with the following signature:
````
internal static bool CmdFoo(GameClient invoker, string invocation) { return true; }
````
The entrypoint -must- begin with Cmd or the method loader won't be able to associate it with your command definition.