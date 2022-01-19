using System.Collections.Generic;
using System.Timers;

namespace inspiral
{
	internal class BalanceBuilder : GameComponentBuilder
	{
		public BalanceBuilder()
		{
			ComponentType = typeof(BalanceComponent);
			schemaFields = new List<DatabaseField>()
			{
				Field.Id,
				Field.Parent,
				Field.Dummy
			};
		}
		internal override GameComponent MakeComponent()
		{
			return new BalanceComponent();
		}
	}

	internal class BalanceComponent : GameComponent 
	{
		private Dictionary<string, Timer> offBalanceTimers = new Dictionary<string, Timer>();
		internal override void InitializeComponent()
		{
			AddBalanceTimer("poise");
			AddBalanceTimer("concentration");
		}

		internal Timer AddBalanceTimer(string balance)
		{
			Timer balTimer = new Timer();
			balTimer.Enabled = false;
			balTimer.Elapsed += (sender, e) => ResetBalance(sender, e, balance);
			balTimer.AutoReset = true;
			balTimer.Interval = 1;
			offBalanceTimers.Add(balance, balTimer);
			return balTimer;
		}
		internal bool OnBalance(string balance)
		{
			return !offBalanceTimers.ContainsKey(balance) || !offBalanceTimers[balance].Enabled;
		}
		internal void KnockBalance(string balance, int msKnock)
		{
			Timer balTimer;
			if(offBalanceTimers.ContainsKey(balance))
			{
				balTimer = offBalanceTimers[balance];
			}
			else
			{
				balTimer = AddBalanceTimer(balance);
			}
			balTimer.Interval += (msKnock-1); // -1 because Interval resets to 1 (cannot be 0).
			balTimer.Enabled = true;
			double secondsLeft = System.Math.Truncate(10 * balTimer.Interval) / 10000; // ms to s, truncating to 2 places
			string secondsString = secondsLeft == 1 ? "second" : "seconds";
			WriteLine($"Your {balance} is lost for {secondsLeft} {secondsString}!");
		}
		private void ResetBalance(object? sender, ElapsedEventArgs e, string balance)
		{
			offBalanceTimers[balance].Enabled = false;
			offBalanceTimers[balance].Interval = 1;
			WriteLine($"You have recovered your {balance}.", true);
		}
		internal override string GetPrompt()
		{
			string p = "";
			foreach(KeyValuePair<string, Timer> bal in offBalanceTimers)
			{
				p += bal.Value.Enabled ? '-' : bal.Key[0];
			}
			return $"{Colours.Fg("[", GetColour(Text.ColourDefaultPrompt))}{Colours.Fg(p, GetColour(Text.ColourDefaultHighlight))}{Colours.Fg("]", GetColour(Text.ColourDefaultPrompt))}";
		}
	}
}
