using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho
{
	public static class Settings
	{
		[ServerVar("gw_bots-per", Help = "Bots per hider")]
		public static int BotsPerHider { get; set; } = 4;

		[ServerVar("gw_hunter-ratio", Help = "1 in X players are hunters")]
		public static int HunterPerHider { get; set; } = 2;

		[ServerVar("gw_players-required", Help = "Players required to play the game")]
		public static int PlayersRequired { get; set; } = 2;
	}
}
