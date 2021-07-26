using guesswho.walker;
using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho.skills
{
	public class CrowdSkill : BaseSkill
	{
		public override float SkillDuration => 16;
		public override string Name => "Crowd";
		List<Walker> walkers = new();

		public override void OnUse(Player owner)
		{
			base.OnUse(owner);

			if (Host.IsClient)
				return;

			for(int i = 0; i < 6; i++)
			{
				Vector3? wPos = NavMesh.GetPointWithinRadius(Owner.Position, 50, 100);
				
				Walker nWalker = new();
				nWalker.Position = wPos.Value;
				walkers.Add(nWalker);
			}

			Walker rWalker = walkers[Rand.Int(0, walkers.Count - 1)];
			rWalker.SwapOutfits(Owner);
			
			Vector3 ourPos = Owner.Position;
			Vector3 walkerPos = rWalker.Position;
			Owner.Position = walkerPos;
			rWalker.Position = ourPos;
		}

		public override void Reset()
		{
			if (!IsActive || Host.IsClient) 
				return;

			walkers.ForEach((x) => x.Delete());
		}
	}
}
