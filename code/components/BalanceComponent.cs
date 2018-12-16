using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Timers;

namespace inspiral
{
	internal static partial class Components
	{
		internal const string Balance = "balance";
	}
	internal class BalanceBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Components.Balance;
		internal override GameComponent Build()
		{
			return new BalanceComponent();
		}
	}
	class BalanceComponent : GameComponent 
	{
		private Dictionary<string, Timer> offBalanceTimers = new Dictionary<string, Timer>();

		internal BalanceComponent()
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
			Timer balTimer = null;
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
			parent.WriteLine($"You are knocked off {balance} for {Math.Truncate((float)balTimer.Interval / 1000.0f)} seconds!");
		}
		private void ResetBalance(object sender, ElapsedEventArgs e, string balance)
		{
			offBalanceTimers[balance].Enabled = false;
			offBalanceTimers[balance].Interval = 1;
			parent.WriteLine($"You have recovered your {balance}.", true);
		}
		internal string GetPrompt()
		{
			string reply = "";
			foreach(KeyValuePair<string, Timer> bal in offBalanceTimers)
			{
				reply += bal.Value.Enabled ? '-' : bal.Key[0];
			}
			return reply;
		}
	}
}
