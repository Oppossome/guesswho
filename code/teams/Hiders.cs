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

		public override float SkillRechargeRate(Player ply)
		{
			if (Host.IsClient)
				return 1;

			float distance = 0;

			foreach(Client client in Client.All)
			{
				Player hunter = client.Pawn as Player;

				if (hunter.Team is not Hunters && hunter.LifeState != LifeState.Alive)
					continue;

				float pDist = Vector3.DistanceBetween(ply.Position, hunter.Position);
				if (distance < pDist) distance = pDist;
			}

			float delta = 1 - Math.Clamp(distance / 250, 0, 1);
			return 2 + 8 * delta;
		}

		public override void OnPlayerRespawned(Player ply)
		{
			ply.Animator = null;
			ply.ResetAnimParams();
		}
	}
}
