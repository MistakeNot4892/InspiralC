using System.Collections.Generic;

namespace inspiral
{
	internal partial class CommandModule : GameModule
	{
		internal void CmdSet(GameObject invoker, CommandData cmd)
		{
			if(cmd.objTarget == null || cmd.strArgs.Length < 3)
			{
				invoker.SendLine("Usage: SET <target> <field> <value>");
				return;
			}
			GameObject editing = invoker.FindGameObjectNearby(cmd.objTarget);
			if(editing == null)
			{
				invoker.SendLine("Cannot find object to modify.");
				return;
			}

			// Do the actual edit.
			string field = cmd.strArgs[1].ToLower();
			string value = "";
			for(int i = 2;i<cmd.strArgs.Length;i++)
			{
				value += $" {cmd.strArgs[i]}";
			}
			value = value.Trim();

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
					GenderObject newGender = Modules.Gender.GetByTerm(value);
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
						if(Modules.Components.builders[comp.Key].editableFields != null && 
							Modules.Components.builders[comp.Key].editableFields.Contains(field))
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
				invoker.WriteLine($"Unknown field '{field}' of object {editing.name}#{editing.id} ({editing.GetShort()}). Check that the object has the component and field that you are trying to edit.");
			}
			else if(invalidValue != null)
			{
				if(invalidValue != "")
				{
					invoker.WriteLine($"Invalid value '{value}' for field '{field}' of object {editing.name}#{editing.id} ({editing.GetShort()}). {invalidValue}");
				}
				else
				{
					invoker.WriteLine($"Invalid value '{value}' for field '{field}' of object {editing.name}#{editing.id} ({editing.GetShort()}).");
				}
			}
			else
			{
				invoker.WriteLine($"Set field '{field}' of object {editing.name}#{editing.id} ({editing.GetShort()}) to '{newVal}'.\nFor reference, previous value was '{lastVal}'.");
			}
			invoker.SendPrompt();
		}
	}
}