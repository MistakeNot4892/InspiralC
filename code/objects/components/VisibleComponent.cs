using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace inspiral
{
	internal partial class ComponentModule : GameModule
	{
		internal List<GameComponent> Visibles => GetComponents<VisibleComponent>();
	}

	internal static partial class Field
	{
		internal static DatabaseField ShortDesc = new DatabaseField(
			"short", "",
			typeof(string), true, true);
		internal static DatabaseField RoomDesc = new DatabaseField(
			"room", "", 
			typeof(string), true, true);
		internal static DatabaseField ExaminedDesc = new DatabaseField(
			"examined", "",
			typeof(string), true, true);
	}

	internal class VisibleBuilder : GameComponentBuilder
	{
		internal override void Initialize()
		{
			ComponentType = typeof(VisibleComponent);
			schemaFields = new List<DatabaseField>()
			{ 
				Field.Parent,
				Field.ShortDesc,
				Field.RoomDesc,
				Field.ExaminedDesc
			};
			base.Initialize();
		}
	}
	internal class VisibleComponent : GameComponent
	{
		internal string shortDescription = "a generic object";
		internal string roomDescription = "A generic object is here.";
		internal string examinedDescription = "This is a generic object. Fascinating stuff.";
		internal void ExaminedBy(GameObject viewer, bool fromInside)
		{
			GameObject? parent = GetParent();
			if(parent == null)
			{
				return;
			}
			string mainDesc = $"{Colours.Fg(parent.GetShortDesc(), viewer.GetColour(Text.ColourDefaultHighlight))}.";
			var getMobComp = parent.GetComponent<MobileComponent>();
			if(getMobComp != null)
			{
				MobileComponent mob = (MobileComponent)getMobComp;
				string startingToken;
				string theyAre;
				string their;
				GenderObject genderObj = Program.Game.Mods.Gender.GetByTerm(parent.GetValue<string>(Field.Gender));
				if(parent == viewer)
				{
					startingToken = "You're";
					theyAre = "You're";
					their = "Your";
				}
				else
				{
					startingToken = "That's";
					theyAre = $"{Text.Capitalize(genderObj.They)} {genderObj.Is}";
					their = Text.Capitalize(genderObj.Their);
				}

				mainDesc = $"{startingToken} {mainDesc}\n{theyAre} a {mob.species}";
				if(viewer == parent)
				{
					mainDesc += $". When people look at you, they see:\n{Text.Capitalize(genderObj.They)} {genderObj.Is} a {mob.species}";
				}
				if(examinedDescription == null || examinedDescription.Length <= 0)
				{
					mainDesc += ".";
				}
				else if(examinedDescription[0] == '.' || examinedDescription[0] == '!' || examinedDescription[0] == '?')
				{
					mainDesc += parent.ApplyStringTokens(examinedDescription);
				}
				else
				{
					mainDesc += $" {parent.ApplyStringTokens(examinedDescription)}";
				}

				List<string> clothing = parent.GetVisibleContents(viewer, false);
				if(clothing.Count > 0)
				{
					mainDesc += $"\n{theyAre} carrying:";
					foreach(string line in clothing)
					{
						mainDesc += $"\n{Text.Capitalize(line)}";
					}
				}
				else
				{
					mainDesc += $"\n{theyAre} completely naked.";
				}
				
				foreach(KeyValuePair<string, GameObject> bp in mob.limbs)
				{
					if(bp.Value == null)
					{
						mainDesc += $"\n{their} {bp.Key} is missing!";
					}
					else
					{
						mainDesc += $"\n{their} {bp.Key} is healthy.";
					}
				}
			}
			else
			{
				mainDesc += $"\n{Colours.Fg(parent.ApplyStringTokens(examinedDescription), parent.GetColour(Text.ColourDefaultSubtle))}";
				if(parent.Contents.Count > 0)
				{
					List<string> roomAppearances = parent.GetVisibleContents(viewer, false);
					if(roomAppearances.Count > 0)
					{
						mainDesc = $"{mainDesc}\n{string.Join(" ", roomAppearances.ToArray())}";
					}
				}
			}

			var getRoomComp = parent.GetComponent<RoomComponent>();
			if(getRoomComp != null)
			{
				RoomComponent roomComp = (RoomComponent)getRoomComp;
				mainDesc = $"{mainDesc}\n{Colours.Fg(roomComp.GetExitString(), viewer.GetColour(Text.ColourDefaultExits))}";
			}

			var getPhysComp = parent.GetComponent<PhysicsComponent>();
			if(getPhysComp != null)
			{
				PhysicsComponent phys = (PhysicsComponent)getPhysComp;
				mainDesc = $"{mainDesc}\n{phys.GetExaminedSummary(viewer)}";
			}
			viewer.WriteLine(mainDesc);
		}
	}
}
