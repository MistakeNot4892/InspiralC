using System.Collections.Generic;

namespace inspiral
{
	internal partial class GameEntity
	{
		internal void ShowToContents(string message)
		{
			foreach(GameEntity obj in contents)
			{
				obj.WriteLine(message, true);
			}
		}
		internal void ShowToContents(GameEntity source, string message)
		{
			ShowToContents(source, message, message);
		}
		internal void ShowToContents(GameEntity source, string message1p, string message3p)
		{
			ShowToContents(source, message1p, message3p, false);
		}

		internal void ShowToContents(GameEntity source, string message1p, string message3p, bool sendPromptToSource)
		{
			if(source.HasComponent<ClientComponent>())
			{
				source.WriteLine(message1p, sendPromptToSource);
			}
			foreach(GameEntity obj in contents)
			{
				if(obj != source && obj.HasComponent<ClientComponent>())
				{
					obj.WriteLine(message3p, true);
				}
			}
		}

		internal virtual void WriteLine(string message)
		{
			WriteLine(message, false);
		}
		internal virtual void WriteLine(string message, bool sendPrompt)
		{
			ClientComponent client = (ClientComponent)GetComponent<ClientComponent>();
			if(client != null && client.client != null)
			{
				client.client.WriteLine(message);
				if(sendPrompt)
				{
					SendPrompt();
				}
			}
		}

		internal virtual void ShowNearby(GameEntity source, GameEntity other,
			string message1p, string message2p, string message3p)
		{
			source.WriteLine(message1p);
			other.WriteLine(message2p);
			ShowNearby(source, message3p, new List<GameEntity>() { source, other });
		}

		internal virtual void ShowNearby(GameEntity source, string message, List<GameEntity> exceptions)
		{
			if(source.location != null)
			{
				foreach(GameEntity obj in source.location.contents)
				{
					if(!exceptions.Contains(obj))
					{
						obj.WriteLine(message, true);

					}
				}
			}
			else
			{
				if(!exceptions.Contains(source))
				{
					source.WriteLine(message, true);
				}
			}
		}

		internal virtual void ShowNearby(GameEntity source, string message)
		{
			ShowNearby(source, message, message);
		}

		internal virtual void ShowNearby(GameEntity source, string message1p, string message3p)
		{
			if(source.location != null)
			{
				source.location.ShowToContents(this, message1p, message3p);
			}
			else
			{
				if(source.HasComponent<ClientComponent>())
				{
					source.WriteLine(message1p);
				}
			}
		}
		internal void Probed(GameEntity invoker)
		{
			string reply = $"{GetShort()} ({name}#{id})";
			reply += "\nContents:";
			if(contents.Count > 0)
			{
				foreach(string visibleThing in GetVisibleContents(invoker, true))
				{
					reply += $"\n- {visibleThing}.";
				}
			}
			else
			{
				reply += "\n- nothing.";
			}
			invoker.WriteLine(reply);
		}

		internal List<string> GetVisibleContents(GameEntity viewer, bool quickView)
		{
			List<string> result = new List<string>();
			if(HasComponent<MobileComponent>())
			{
				if(HasComponent<InventoryComponent>())
				{
					string their = (this == viewer) ? "your" : gender.Their;
					InventoryComponent equip = (InventoryComponent)GetComponent<InventoryComponent>();
					foreach(KeyValuePair<string, GameEntity> equ in equip.carrying)
					{
						if(quickView)
						{
							if(equip.GetWieldableSlots().Contains(equ.Key))
							{
								result.Add($"{equ.Value.GetShort()} ({equ.Value.name}#{equ.Value.id}) ({equ.Key}, wielded)");
							}
							else
							{
								result.Add($"{equ.Value.GetShort()} ({equ.Value.name}#{equ.Value.id}) ({equ.Key}, worn)");
							}
						}
						else
						{
							if(equip.GetWieldableSlots().Contains(equ.Key))
							{
								result.Add($"{equ.Value.GetShort()} in {their} {equ.Key}.");
							}
							else
							{
								result.Add($"{equ.Value.GetShort()} on {their} {equ.Key}.");
							}
						}
					}
				}
			}
			else
			{
				foreach(GameEntity gameObj in contents)
				{
					if(quickView)
					{
						result.Add($"{gameObj.GetShort()} ({gameObj.name}#{gameObj.id})");
					}
					else
					{
						if(gameObj != viewer)
						{
							result.Add(gameObj.GetString<VisibleComponent>(Text.FieldRoomDesc));
						}
					}
				}
			}
			return result;
		}
		internal void ExaminedBy(GameEntity viewer, bool fromInside)
		{
			if(HasComponent<VisibleComponent>())
			{
				VisibleComponent comp = (VisibleComponent)GetComponent<VisibleComponent>();
				comp.ExaminedBy(viewer, fromInside);
			}
			else
			{
				viewer.WriteLine("There is nothing there.");
			}
		}
		internal string GetShort()
		{
			return GetString<VisibleComponent>(Text.FieldShortDesc);
		}
	}
}