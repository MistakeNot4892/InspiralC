namespace inspiral
{
	internal class MobileComponent : GameComponent
	{
		internal string enterMessage = "A generic object enters from the $DIR.";
		internal string leaveMessage = "A generic object leaves to the $DIR.";
		internal string deathMessage = "A generic object lies here, dead.";

		internal MobileComponent()
		{
			key = Components.Mobile;
		}

		internal override void SetValue(int field, string newValue)
		{
			switch(field)
			{
				case Text.FieldEnterMessage:
					leaveMessage = newValue;
					break;
				case Text.FieldLeaveMessage:
					leaveMessage = newValue;
					break;
				case Text.FieldDeathMessage:
					deathMessage = newValue;
					break;
			}
		}
		internal override string GetStringValue(int field)
		{
			switch(field)
			{
				case Text.FieldEnterMessage:
					return enterMessage;
				case Text.FieldLeaveMessage:
					return leaveMessage;
				case Text.FieldDeathMessage:
					return deathMessage;
				default:
					return null;
			}
		}
	}
}
