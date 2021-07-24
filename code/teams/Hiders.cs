using Sandbox;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace guesswho.teams
{
	public class Hiders : BaseTeam
	{
		public override string Name => "Hiders";

		public override void OnPlayerDied(Player ply)
		{
			
		}

		public override void OnPlayerRespawned(Player ply)
		{
			ply.Animator = null;
			ply.ResetAnimParams();
		}
	}
}
