using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class Text
	{
		internal const string CompBodypart = "bodypart";
		internal const string FieldCanGrasp = "cangrasp";
		internal const string FieldCanStand = "canstand";
		internal const string FieldNaturalWeapon = "isnaturalweapon";
		internal const string FieldEquipmentSlots = "equipmentslots";
	}
	internal class BodypartBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Text.CompBodypart;
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_bodyparts WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = $@"components_bodyparts (
				id INTEGER NOT NULL PRIMARY KEY UNIQUE, 
				{Text.FieldCanGrasp}       INTEGER DEFAULT 0,
				{Text.FieldCanStand}       INTEGER DEFAULT 0,
				{Text.FieldNaturalWeapon}  INTEGER DEFAULT 0,
				{Text.FieldEquipmentSlots} TEXT DEFAULT ''
				)";
		internal override string UpdateSchema   { get; set; } = $@"UPDATE components_bodyparts SET 
				{Text.FieldCanGrasp} =       @p1,
				{Text.FieldCanStand} =       @p2,
				{Text.FieldNaturalWeapon} =  @p3,
				{Text.FieldEquipmentSlots} = @p4
				WHERE id = @p0";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_bodyparts (
				id,
				{Text.FieldCanGrasp},
				{Text.FieldCanStand},
				{Text.FieldNaturalWeapon},
				{Text.FieldEquipmentSlots}
				) VALUES (
				@p0, 
				@p1,
				@p2,
				@p3,
				@p4
				);";
		internal override GameComponent Build()
		{
			return new BodypartComponent();
		}
	}
	internal class BodypartComponent : GameComponent
	{
		internal bool canGrasp = false;
		internal bool canStand = false;
		internal bool isNaturalWeapon = false;
		internal List<string> equipmentSlots = new List<string>();
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			canGrasp =        ((long)reader[Text.FieldCanGrasp] == 1);
			canStand =        ((long)reader[Text.FieldCanStand] == 1);
			isNaturalWeapon = ((long)reader[Text.FieldNaturalWeapon] == 1);
			equipmentSlots = JsonConvert.DeserializeObject<List<string>>((string)reader[Text.FieldEquipmentSlots]);
		}
		internal override void ConfigureFromJson(JToken compData)
		{
			if(!JsonExtensions.IsNullOrEmpty(compData[Text.FieldCanGrasp]))
			{
				canGrasp = (bool)compData[Text.FieldCanGrasp];
			}
			if(!JsonExtensions.IsNullOrEmpty(compData[Text.FieldCanStand]))
			{
				canStand = (bool)compData[Text.FieldCanStand];
			}
			if(!JsonExtensions.IsNullOrEmpty(compData[Text.FieldNaturalWeapon]))
			{
				isNaturalWeapon = (bool)compData[Text.FieldNaturalWeapon];
			}
			if(!JsonExtensions.IsNullOrEmpty(compData[Text.FieldEquipmentSlots]))
			{
				foreach(string s in JsonConvert.DeserializeObject<List<string>>(compData[Text.FieldEquipmentSlots].ToString()))
				{
					if(!equipmentSlots.Contains(s))
					{
						equipmentSlots.Add(s);
					}
				}
			}
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", canGrasp);
			command.Parameters.AddWithValue("@p2", canStand);
			command.Parameters.AddWithValue("@p3", isNaturalWeapon);
			command.Parameters.AddWithValue("@p4", JsonConvert.SerializeObject(equipmentSlots));
		}
	}
}