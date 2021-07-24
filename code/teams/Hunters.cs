using guesswho.rounds;
using System;
using System.Collections.Generic;


namespace guesswho.teams
{
	public class Hunters : BaseTeam
	{
		public override string Name => "Hunters";

		public override void OnPlayerDied(Player ply)
		{
			
		}

		public override void OnPlayerRespawned(Player ply)
		{
			
		}

		public override bool ShouldPlayerMove(Player ply)
		{
			return Game.CurrentRound is not HidingRound;
		}
	}
}