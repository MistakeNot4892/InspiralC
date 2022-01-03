using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Timers;

namespace inspiral
{
	internal partial class Text
	{
		internal const string CompBalance = "balance";
	}
	internal class BalanceBuilder : GameComponentBuilder
	{
		internal override string Name { get; set; } = Text.CompBalance;
		internal override string LoadSchema   { get; set; } = "SELECT * FROM components_balance WHERE id = @p0;";
		internal override string TableSchema  { get; set; } = "CREATE TABLE IF NOT EXISTS components_balance ( id INTEGER NOT NULL PRIMARY KEY UNIQUE, placeholder STRING NOT NULL )";
		internal override string InsertSchema { get; set; } = @"INSERT INTO components_balance (
			id, 
			placeholder
			) VALUES (
			@p0,
			@p1
			);";
		internal override string UpdateSchema   { get; set; } = $@"UPDATE components_balance SET 
			placeholder = @p1 
			WHERE id = @p0;";
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
			isPersistent = false;
			AddBalanceTimer("poise");
			AddBalanceTimer("concentration");
		}
		internal override void AddCommandParameters(SQLiteCommand command) // TODO: store balance configuration? does this need to be a component?
		{
			command.Parameters.AddWithValue("@p0", parent.id);
			command.Parameters.AddWithValue("@p1", "placeholder");
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
			double secondsLeft = Math.Truncate(10 * balTimer.Interval) / 10000; // ms to s, truncating to 2 places
			string secondsString = secondsLeft == 1 ? "second" : "seconds";
			parent.WriteLine($"Your {balance} is lost for {secondsLeft} {secondsString}!");
		}
		private void ResetBalance(object sender, ElapsedEventArgs e, string balance)
		{
			offBalanceTimers[balance].Enabled = false;
			offBalanceTimers[balance].Interval = 1;
			parent.WriteLine($"You have recovered your {balance}.", true);
		}
		internal override string GetPrompt()
		{
			string p = "";
			foreach(KeyValuePair<string, Timer> bal in offBalanceTimers)
			{
				p += bal.Value.Enabled ? '-' : bal.Key[0];
			}
			return $"{Colours.Fg("[", Colours.Yellow)}{Colours.Fg(p, Colours.BoldWhite)}{Colours.Fg("]", Colours.Yellow)}";
		}
	}
}
