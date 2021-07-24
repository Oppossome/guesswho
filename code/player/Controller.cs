using guesswho.teams;
using Sandbox;
using System;
using System.Collections.Generic;


namespace guesswho.player
{
	public class Controller : WalkController
	{

		//TODO: Implement hider walker walkspeed match
		bool ShouldMove()
		{
			if (this.Pawn is not Player ply)
				return true;

			if (ply.Team is BaseTeam team)
				return team.ShouldPlayerMove(ply);

			return true;
		}

		public override void Accelerate(Vector3 wishdir, float wishspeed, float speedLimit, float acceleration)
		{
			if (ShouldMove())
				base.Accelerate(wishdir, wishspeed, speedLimit, acceleration);
		}
	}
}
