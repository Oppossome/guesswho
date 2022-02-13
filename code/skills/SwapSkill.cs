using guesswho.walker;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace guesswho.skills
{
	public class SwapSkill : BaseSkill
	{
		public override float SkillDuration => 5;
		public override string Name => "Swap";

		public override void OnUse(Player owner)
		{
			base.OnUse(owner);

			if (Host.IsClient)
				return;

			List<Walker> walkers = Entity.All.OfType<Walker>().ToList();
			Walker rWalker = walkers[Rand.Int(0, walkers.Count - 1)];
			rWalker.SwapOutfits(Owner);

			Vector3 ourPosition = Owner.Position;
			Vector3 walkerPosition = rWalker.Position;
			Owner.Position = walkerPosition;
			rWalker.Position = ourPosition;
		}
	}
}
