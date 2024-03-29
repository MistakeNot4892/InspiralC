using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Physics => Repositories.Components.GetComponents("physics");
	}

	internal static partial class Field
	{
		internal static DatabaseField Length = new DatabaseField(
			"length", 1,
			typeof(int), true, true, false);
		internal static DatabaseField Width = new DatabaseField(
			"width", 1,
			typeof(int), true, true, false);
		internal static DatabaseField Height = new DatabaseField(
			"height", 1,
			typeof(int), true, true, false);
		internal static DatabaseField Density = new DatabaseField(
			"density", 1.0,
			typeof(double), true, true, false);
		internal static DatabaseField StrikeArea = new DatabaseField(
			"strikearea", 1.0,
			typeof(double), true, true, false);
		internal static DatabaseField Edged = new DatabaseField(
			"edged", 0,
			typeof(int), true, true, false);
	}
	internal class PhysicsBuilder : GameComponentBuilder
	{
		public PhysicsBuilder()
		{
			tableName = "physics";
			ComponentId = "physics";
			schemaFields = new List<DatabaseField>() 
			{ 
				Field.Id,
				Field.Parent,
				Field.ComponentId,
				Field.Length, 
				Field.Width, 
				Field.Height, 
				Field.Density, 
				Field.StrikeArea, 
				Field.Edged
			};
		}
		internal override GameComponent MakeComponent()
		{
			return new PhysicsComponent();
		}
	}
	class PhysicsComponent : GameComponent
	{
		internal int length = 10;         // cm
		internal int width = 10;          // cm
		internal int height = 10;         // cm
		internal int volume = 1;          // cm^3
		internal double density = 1.0;     // multiplier for mass to weight calc.
		internal double mass = 1.0;        // kg
		internal double surfaceArea = 1.0; // cm^2
		internal double contactArea = 1.0; // cm^2
		internal double strikeArea = -1.0; // cm^2
		internal int edged = 0;
		internal void UpdateValues()
		{
			volume = length * width * height;
			mass = (volume * density)/1000;
			surfaceArea = 2*((width*length)+(height*length)+(height*width));
			contactArea = (width * height);
			if(strikeArea < 0)
			{
				strikeArea = contactArea;
			}
		}
		internal bool SetDensity(double newValue)
		{
			if(density != newValue)
			{
				density = newValue;
				UpdateValues();
				return true;
			}
			return false;
		}
		private string FormatKilogramsForDisplay(double kg)
		{
			if(mass <= 0.001)
			{
				return "1 gram";
			}
			else if(mass < 1.0)
			{
				return $"{(int)(mass * 1000)} grams";
			}
			else if(mass == 1.0)
			{
				return "1 kilogram";
			}
			else if(mass < 1000.0)
			{
				return $"{mass} kilograms";
			}
			else if(mass == 1000.0)
			{
				return $"1 ton";
			}
			else
			{
				return $"{mass/1000.0} tons";
			}
		}

		private string FormatCentimetersForDisplay(int cm)
		{
			if(cm < 100)
			{
				return $"{cm} centimeters";
			}
			else if(cm == 100)
			{
				return $"1 meter";
			}
			else if(cm < 100000)
			{
				return $"{(double)(cm/100.0)} meters";
			}
			else if(cm == 100000)
			{
				return "1 kilometer";
			}
			else
			{
				return $"{(double)(cm / 100000.0)} kilometers";
			}
		}
		internal string GetExaminedSummary(GameObject viewer)
		{
			GameObject? parent = GetParent();
			if(parent == null)
			{
				return "";
			}

			string result = "";
			string You;
			string weigh;
			string are;
			if(viewer == parent)
			{
				You = "You";
				weigh = "weigh";
				are = "are";
			}
			else
			{
				GenderObject genderObj = Modules.Gender.GetByTerm(parent.GetValue<string>(Field.Gender));
				You = Text.Capitalize(genderObj.They);
				are = genderObj.Is;
				if(are == "is")
				{
					weigh = "weighs";
				}
				else
				{
					weigh = "weigh";
				}				
			}
			result = $"{You} {are} around {FormatCentimetersForDisplay(height)} tall, {FormatCentimetersForDisplay(width)} broad and {FormatCentimetersForDisplay(length)} deep.\n{You} {weigh} around {FormatKilogramsForDisplay(mass)}.";
			return result;
		}

		internal double GetImpactPenetration(double velocity, double tensileResistance)
		{
			return (0.5 * mass * (velocity * velocity))/(tensileResistance * (strikeArea * 0.01));
		}
	}
}
