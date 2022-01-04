using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace inspiral
{

	internal static partial class Modules
	{
		internal static TemplateModule Templates;
	}
	internal class GameObjectTemplate
	{
		internal string templateName;
		internal string objectGender = Text.GenderInanimate;
		internal List<string> aliases = new List<string>();
		internal List<JToken> components = new List<JToken>();
		internal GameObjectTemplate(string input)
		{
			JObject r = JObject.Parse(input);
			templateName = (string)r["templateid"];
			if(!JsonExtensions.IsNullOrEmpty(r["gender"]))
			{
				objectGender = r["gender"].ToString();
			}
			if(!JsonExtensions.IsNullOrEmpty(r["components"]))
			{
				foreach(JToken comp in r["components"])
				{
					components.Add(comp);
				}
			}
			if(!JsonExtensions.IsNullOrEmpty(r["aliases"]))
			{
				foreach(string s in r["aliases"].Select(t => (string)t).ToList())
				{
					aliases.Add(s);
				}
			}
			Modules.Templates.Register(this);
		}
		internal void CopyTo(GameObject copyingTo)
		{
			foreach(string s in aliases)
			{
				copyingTo.aliases.Add(s);
			}
			copyingTo.name = copyingTo.aliases.First();
			copyingTo.gender = Modules.Gender.GetByTerm(objectGender);
			foreach(JProperty s in components)
			{
				copyingTo.AddComponent(Game.GetTypeFromString(s.Name), s);
			}
			Game.Objects.QueueForUpdate(copyingTo);
		}
	}
	internal class TemplateModule : GameModule
	{
		private Dictionary<string, GameObjectTemplate> templates = new Dictionary<string, GameObjectTemplate>();
		internal override void Initialize()
		{
			Modules.Templates = this;
			Debug.WriteLine("Loading templates.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data/definitions/templates", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Debug.WriteLine($"- Loading template definition {f.File}.");
				try
				{
					new GameObjectTemplate(File.ReadAllText(f.File));
				}
				catch(System.Exception e)
				{
					Debug.WriteLine($"Exception when loading template from file {f.File} - {e.Message}");
				}
			}
			Debug.WriteLine("Done.");
		}
		internal GameObject Instantiate(string template)
		{
			GameObject creating = null;
			GameObjectTemplate temp = Get(template);
			if(temp != null)
			{
				creating = (GameObject)Game.Objects.CreateNewInstance(false);
				temp.CopyTo(creating);
				Game.Objects.AddDatabaseEntry(creating);
			}
			return creating;
		}
		internal void Register(GameObjectTemplate template)
		{
			templates.Add(template.templateName, template);
		}
		internal GameObjectTemplate Get(string temp)
		{
			if(templates.ContainsKey(temp))
			{
				return templates[temp];
			}
			return null;
		}
		internal List<string> GetTemplateNames()
		{
			List<string> names = new List<string>();
			foreach(KeyValuePair<string, GameObjectTemplate> temp in templates)
			{
				names.Add(temp.Key);
			}
			return names;
		}
	}
}