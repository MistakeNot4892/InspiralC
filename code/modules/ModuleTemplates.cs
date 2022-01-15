using System.Collections.Generic;
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
			templateName = r["templateid"].ToString();
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
				//copyingTo.AddComponent(Game.GetTypeFromString(s.Name), s);
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
			Game.LogError("Loading templates.");
			foreach (var f in (from file in Directory.EnumerateFiles(@"data/definitions/templates", "*.json", SearchOption.AllDirectories) select new { File = file }))
			{
				Game.LogError($"- Loading template definition {f.File}.");
				try
				{
					new GameObjectTemplate(File.ReadAllText(f.File));
				}
				catch(System.Exception e)
				{
					Game.LogError($"Exception when loading template from file {f.File} - {e.Message}");
				}
			}
			Game.LogError("Done.");
		}
		internal GameObject Instantiate(string template)
		{
			GameObject creating = null;
			GameObjectTemplate temp = GetTemplate(template);
			if(temp != null)
			{
				creating = (GameObject)Game.Objects.CreateNewInstance();
				temp.CopyTo(creating);
			}
			return creating;
		}
		internal void Register(GameObjectTemplate template)
		{
			templates.Add(template.templateName, template);
		}
		internal GameObjectTemplate GetTemplate(string temp)
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