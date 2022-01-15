using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Physics => GetComponents<PhysicsComponent>();
	}

	internal static partial class Field
	{
		internal const string Length =  "length";
		internal const string Width =   "width";
		internal const string Height =  "height";
		internal const string Density = "density";
		internal const string StrikeArea = "strikearea";
		internal const string Edged = "edged";
	}
	internal class PhysicsBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(PhysicsComponent);
			schemaFields = new Dictionary<string, (System.Type, string, bool, bool)>()
			{
				{ Field.Length,     (typeof(int),    "1",   true, true) },
				{ Field.Width,      (typeof(int),    "1",   true, true) },
				{ Field.Height,     (typeof(int),    "1",   true, true) },
				{ Field.Density,    (typeof(double), "1.0", true, true) },
				{ Field.StrikeArea, (typeof(double), "1.0", true, true) }, 
				{ Field.Edged,      (typeof(int),    "0",   true, true) }
			};
			base.Initialize();
		}
	}
	class PhysicsComponent : GameComponent
	{
		internal long length = 10;         // cm
		internal long width = 10;          // cm
		internal long height = 10;         // cm
		internal long volume = 1;          // cm^3
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
		internal override bool SetValue(string key, string newValue)
		{
			try
			{
				if(key == Field.Density)
				{
					return SetDensity(System.Convert.ToDouble(newValue));
				}
				else
				{
					return SetValue(key, (long)System.Convert.ToInt64(newValue));
				}
			}
			catch(System.Exception e)
			{
				Game.LogError($"Conversion exception in physics SetValue(): {e.Message}");
				return false; 
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
		internal override bool SetValue(string field, long newValue)
		{
			newValue = System.Math.Clamp(newValue, 1, 100000000);
			bool success = false;
			switch(field)
			{
				case Field.Length:
					if(length != newValue)
					{
						length = newValue;
						success = true;
					}
					break;
				case Field.Width:
					if(width != newValue)
					{
						width = newValue;
						success = true;
					}
					break;
				case Field.Height:
					if(height != newValue)
					{
						height = newValue;
						success = true;
					}
					break;
				case Field.Density:
					if(density != newValue)
					{
						density = newValue;
						success = true;
					}
					break;
			}
			if(success)
			{
				UpdateValues();
			}
			return success;
		}
		internal override long GetLong(string field)
		{
			switch(field)
			{
				case Field.Width:
					return width;
				case Field.Length:
					return length;
				case Field.Height:
					return height;
				default:
					return 0;
			}
		}
		internal override string GetString(string field)
		{
			switch(field)
			{
				case Field.Width:
				case Field.Length:
				case Field.Height:
					return GetLong(field).ToString();
				case Field.Density:
					return $"{density}";
				case Field.StrikeArea:
					return $"{density}";
				case Field.Edged:
					return $"{density}";
			}
			return null;
		}

		private string FormatKilogramsForDisplay(double kg)
		{
			if(mass <= 0.001)
			{
				return "1 gram";
			}
			else if(mass < 1.0)
			{
				return $"{(long)(mass * 1000)} grams";
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

		private string FormatCentimetersForDisplay(long cm)
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
				You = Text.Capitalize(parent.gender.They);
				are = parent.gender.Is;
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
		internal override Dictionary<string, object> GetSaveData()
		{
			Dictionary<string, object> saveData = base.GetSaveData();
			saveData.Add(Field.Length,     length);
			saveData.Add(Field.Width,      width);
			saveData.Add(Field.Height,     height);
			saveData.Add(Field.Density,    density);
			saveData.Add(Field.StrikeArea, strikeArea);
			saveData.Add(Field.Edged,      edged);
			return saveData;
		}
		internal override void CopyFromRecord(DatabaseRecord record) 
		{
			base.CopyFromRecord(record);
			length =     (long)record.fields[Field.Length];
			width =      (long)record.fields[Field.Width];
			height =     (long)record.fields[Field.Height];
			density =    (double)record.fields[Field.Density];
			strikeArea = (double)record.fields[Field.StrikeArea];
			edged =      (int)record.fields[Field.Edged];
			UpdateValues();
		}
	}
}
