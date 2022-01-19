using System.Collections.Generic;

namespace inspiral
{
	internal class CommandSet : GameCommand
	{
		internal override void Initialize()
		{
			Aliases.Add("set");
			Aliases.Add("vs");
			Description = "Modifies the editable fields of an object.";
			Usage = "set [object name or id] [object field] [new value]";
			SkipArticles = false;
			SkipTokenQualifiers = true;
		}
		internal override void InvokeCommand(GameObject invoker, CommandData cmd)
		{
			if(cmd.ObjTarget == null || cmd.StrArgs.Length < 3)
			{
				invoker.WriteLine("Usage: SET <target> <field> <value>");
				return;
			}
			GameObject? editing = invoker.FindGameObjectNearby(cmd.ObjTarget);
			if(editing == null)
			{
				invoker.WriteLine("Cannot find object to modify.");
				return;
			}

			// Do the actual edit.
			DatabaseField field = Field.GetFieldFromName(cmd.StrArgs[1].ToLower());
			string value = "";
			for(int i = 2;i<cmd.StrArgs.Length;i++)
			{
				value += $" {cmd.StrArgs[i]}";
			}
			value = value.Trim();

			string? newVal = value;
			string? lastVal = "";
			string? invalidValue = null;
			bool unknownValue = false;
	
			if(field.IsField(Field.Name))
			{
				lastVal = editing.GetValue<string>(Field.Name);
				editing.SetValue(Field.Name, value);
			}
			else if(field.IsField(Field.Gender))
			{
				lastVal = editing.GetValue<string>(Field.Gender);
				GenderObject? newGender = Program.Game.Mods.Gender.GetByTerm(value);
				if(newGender == null)
				{
					invalidValue = "Non-existent gender.";
				}
				else
				{
					editing.SetValue(Field.Gender, newGender.Term);
					newVal = editing.GetValue<string>(Field.Gender);
				}
			}
			else
			{
				unknownValue = true;
				foreach(KeyValuePair<System.Type, GameComponent> comp in editing.Components)
				{
					if(Program.Game.Mods.Components.builders[comp.Key].editableFields != null && 
						Program.Game.Mods.Components.builders[comp.Key].editableFields.Contains(field))
					{
						unknownValue = false;
						lastVal = comp.Value.GetValue<string>(field);
						if(!comp.Value.SetValueOfEditableField(field, value))
						{
							invalidValue = "Invalid field.";
						}
						newVal = comp.Value.GetValue<string>(field);
					}
				}
			}

			if(unknownValue)
			{
				invoker.WriteLine($"Unknown field '{field}' of object {editing.GetValue<string>(Field.Name)}#{editing.GetValue<ulong>(Field.Id)} ({editing.GetShortDesc()}). Check that the object has the component and field that you are trying to edit.");
			}
			else if(invalidValue != null)
			{
				if(invalidValue != "")
				{
					invoker.WriteLine($"Invalid value '{value}' for field '{field}' of object {editing.GetValue<string>(Field.Name)}#{editing.GetValue<ulong>(Field.Id)} ({editing.GetShortDesc()}). {invalidValue}");
				}
				else
				{
					invoker.WriteLine($"Invalid value '{value}' for field '{field}' of object {editing.GetValue<string>(Field.Name)}#{editing.GetValue<ulong>(Field.Id)} ({editing.GetShortDesc()}).");
				}
			}
			else
			{
				invoker.WriteLine($"Set field '{field}' of object {editing.GetValue<string>(Field.Name)}#{editing.GetValue<ulong>(Field.Id)} ({editing.GetShortDesc()}) to '{newVal}'.\nFor reference, previous value was '{lastVal}'.");
			}
		}
	}
}