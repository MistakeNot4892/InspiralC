using System.Data.SQLite;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Physics => GetComponents(Text.CompPhysics);
	}

	internal static partial class Text
	{
		internal const string CompPhysics =  "physics";
		internal const string FieldLength =  "length";
		internal const string FieldWidth =   "width";
		internal const string FieldHeight =  "height";
		internal const string FieldDensity = "density";
	}
	internal class PhysicsBuilder : GameComponentBuilder
	{
		internal override List<string> editableFields { get; set; } = new List<string>() {Text.FieldLength, Text.FieldWidth, Text.FieldHeight, Text.FieldDensity};
		internal override List<string> viewableFields { get; set; } = new List<string>() {Text.FieldLength, Text.FieldWidth, Text.FieldHeight, Text.FieldDensity};

		internal override string Name { get; set; } = Text.CompPhysics;
		internal override GameComponent Build()
		{
			return new PhysicsComponent();
		}
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_physics WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = $@"components_physics (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				{Text.FieldLength}  INTEGER DEFAULT 1,
				{Text.FieldWidth}   INTEGER DEFAULT 1,
				{Text.FieldHeight}  INTEGER DEFAULT 1,
				{Text.FieldDensity} DOUBLE DEFAULT 1.0
				)";
		internal override string UpdateSchema   { get; set; } = $@"UPDATE components_physics SET 
				{Text.FieldLength} =  @p1, 
				{Text.FieldWidth} =   @p2, 
				{Text.FieldHeight} =  @p3, 
				{Text.FieldDensity} = @p4 
				WHERE id = @p0";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_physics (
				id,
				{Text.FieldLength},
				{Text.FieldWidth},
				{Text.FieldHeight},
				{Text.FieldDensity}
				) VALUES (
				@p0, 
				@p1,
				@p2,
				@p3,
				@p4
				);";
	}
	class PhysicsComponent : GameComponent
	{
		internal long length = 10;      // cm
		internal long width = 10;       // cm
		internal long height = 10;      // cm
		internal long volume = 1;      // cm^3
		internal double density = 1.0;     // multiplier for mass to weight calc.
		internal double mass = 1.0;        // kg
		internal double surfaceArea = 1.0; // cm^2
		internal double contactArea = 1.0; // cm^2
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			length =  (long)reader[Text.FieldLength];
			width =   (long)reader[Text.FieldWidth];
			height =  (long)reader[Text.FieldHeight];
			density = (double)reader[Text.FieldDensity];
			UpdateValues();
		}
		internal override void ConfigureFromJson(JToken compData)
		{
			if(!JsonExtensions.IsNullOrEmpty(compData[Text.FieldLength]))
			{
				length =  (long)compData[Text.FieldLength];
			}
			if(!JsonExtensions.IsNullOrEmpty(compData[Text.FieldWidth]))
			{
				width =   (long)compData[Text.FieldWidth];
			}
			if(!JsonExtensions.IsNullOrEmpty(compData[Text.FieldHeight]))
			{
				height =  (long)compData[Text.FieldHeight];
			}
			if(!JsonExtensions.IsNullOrEmpty(compData[Text.FieldDensity]))
			{
				density = (double)compData[Text.FieldDensity];
			}
			UpdateValues();
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", length);
			command.Parameters.AddWithValue("@p2", width);
			command.Parameters.AddWithValue("@p3", height);
			command.Parameters.AddWithValue("@p4", density);
		}
		private void UpdateValues()
		{
			volume = length * width * height;
			mass = (volume * density)/1000;
			surfaceArea = 2*((width*length)+(height*length)+(height*width));
			contactArea = (width * height);
		}

		internal override bool SetValue(string key, string newValue)
		{
			try
			{
				if(key == Text.FieldDensity)
				{
					return SetDensity(Convert.ToDouble(newValue));
				}
				else
				{
					return SetValue(key, (long)Convert.ToInt64(newValue));
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine($"Conversion exception in physics SetValue(): {e.Message}");
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
			newValue = Math.Clamp(newValue, 1, 100000000);
			bool success = false;
			switch(field)
			{
				case Text.FieldLength:
					if(length != newValue)
					{
						length = newValue;
						success = true;
					}
					break;
				case Text.FieldWidth:
					if(width != newValue)
					{
						width = newValue;
						success = true;
					}
					break;
				case Text.FieldHeight:
					if(height != newValue)
					{
						height = newValue;
						success = true;
					}
					break;
				case Text.FieldDensity:
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
				case Text.FieldWidth:
					return width;
				case Text.FieldLength:
					return length;
				case Text.FieldHeight:
					return height;
				default:
					return 0;
			}
		}
		internal override string GetString(string field)
		{
			switch(field)
			{
				case Text.FieldWidth:
				case Text.FieldLength:
				case Text.FieldHeight:
					return GetLong(field).ToString();
				case Text.FieldDensity:
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
				You = Text.Capitalize(parent.gender.He);
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
	}
}