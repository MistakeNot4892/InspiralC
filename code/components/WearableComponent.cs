using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Wearables =>  GetComponents(Text.CompWearable);
	}
	internal static partial class Text
	{
		internal const string CompWearable = "wearable";
		internal const string FieldWearableSlots = "wearable_slots";
	}
	internal class WearableBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Text.CompWearable;
		internal override List<string> editableFields { get; set; } = new List<string>() {Text.FieldWearableSlots};
		internal override List<string> viewableFields { get; set; } = new List<string>() {Text.FieldWearableSlots};
		internal override GameComponent Build()
		{
			return new WearableComponent();
		}
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_wearable WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = $@"components_wearable (
			id INTEGER NOT NULL PRIMARY KEY UNIQUE,
			{Text.FieldWearableSlots} TEXT DEFAULT ''
			)";
		internal override string UpdateSchema { get; set; } = $@"UPDATE components_wearable SET 
			{Text.FieldWearableSlots} = @p1 
			WHERE id = @p0;";
		internal override string InsertSchema { get; set; } = $@"INSERT INTO components_wearable (
			id, 
			{Text.FieldWearableSlots}
			) VALUES (
			@p0,
			@p1 
			);";
	}
	class WearableComponent : GameComponent 
	{
		internal List<string> wearableSlots = new List<string>();
		internal override void InstantiateFromRecord(SQLiteDataReader reader) 
		{
			wearableSlots = JsonConvert.DeserializeObject<List<string>>(reader[Text.FieldWearableSlots].ToString());
		}
		internal override void AddCommandParameters(SQLiteCommand command) 
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", JsonConvert.SerializeObject(wearableSlots));
		}
		internal override void ConfigureFromJson(JToken compData)
		{
			foreach(string s in compData["worn"].Select(t => (string)t).ToList())
			{
				wearableSlots.Add(s);
			}
		}
	}
}
