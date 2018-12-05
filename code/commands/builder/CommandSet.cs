using System.Collections.Generic;

namespace inspiral
{
	internal static partial class Commands
	{
		internal static CommandSet Set = new CommandSet();
	}
	class CommandSet : GameCommand
	{
		internal override string Usage { get; set; } = "set [object name or id] [object field] [new value]";
		internal CommandSet() 
		{
			commandString = "set";
			aliases = new List<string>() {"vs"};
		}
		internal override bool Invoke(GameClient invoker, string invocation)
		{
			string[] tokens = invocation.Split(" ");
			if(tokens.Length < 3)
			{
				invoker.SendLineWithPrompt("Usage: SET <target> <field> <value>");
				return true;
			}
			GameObject editing = invoker.shell.FindGameObjectNearby(tokens[0].ToLower());
			if(editing == null)
			{
				invoker.SendLineWithPrompt("Cannot find object to modify.");
				return true;
			}

			// Do the actual edit.
			string value = invocation.Substring(tokens[0].Length + tokens[1].Length + 2);
			string field = tokens[1].ToLower();
			string lastVal = "";
			string newVal = value;
			string invalidValue = null;
			bool unknownValue = false;
	
			switch(field)
			{
				case "name":
					lastVal = editing.name;
					editing.name = value;
					break;

				case "gender":
					lastVal = $"{editing.gender.Term}";
					GenderObject newGender = Gender.GetByTerm(value);
					if(newGender == null)
					{
						invalidValue = "Non-existent gender.";
					}
					else
					{
						editing.gender = newGender;
						newVal = $"{editing.gender.Term}";
					}
					break;

				default:
					unknownValue = true;
					foreach(KeyValuePair<string, GameComponent> comp in editing.components)
					{
						if(Components.builders[comp.Key].editableFields != null && 
							Components.builders[comp.Key].editableFields.Contains(field))
						{
							unknownValue = false;
							lastVal = comp.Value.GetString(field);
							invalidValue = comp.Value.SetValueOfEditableField(field, value);
							newVal = comp.Value.GetString(field);
						}
					}
					break;
			}

			if(unknownValue)
			{
				invoker.SendLineWithPrompt($"Unknown field '{field}' of object #{editing.id} ({editing.GetString(Components.Visible, Text.FieldShortDesc)}. Check that the object has the component and field that you are trying to edit.");
			}
			else if(invalidValue != null)
			{
				if(invalidValue != "")
				{
					invoker.SendLineWithPrompt($"Invalid value '{value}' for field '{field}' of object #{editing.id} ({editing.GetString(Components.Visible, Text.FieldShortDesc)}). {invalidValue}");
				}
				else
				{
					invoker.SendLineWithPrompt($"Invalid value '{value}' for field '{field}' of object #{editing.id} ({editing.GetString(Components.Visible, Text.FieldShortDesc)}).");
				}
			}
			else
			{
				invoker.SendLineWithPrompt($"Set field '{field}' of object #{editing.id} ({editing.GetString(Components.Visible, Text.FieldShortDesc)}) to '{newVal}'.\nFor reference, previous value was '{lastVal}'.");
			}
			return true;
		}
	}
}