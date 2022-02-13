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

			for(int i = 0; i < 3; i++)
			{
				Vector3? wPosition = NavMesh.GetPointWithinRadius(Owner.Position, 50, 100);
				
				if(wPosition is Vector3 rPosition)
				{
					Walker nWalker = new();
					nWalker.Position = rPosition;
					walkers.Add(nWalker);
				}
			}

			Walker rWalker = walkers[Rand.Int(0, walkers.Count - 1)];
			rWalker.SwapOutfits(Owner);
			
			Vector3 ourPosition = Owner.Position;
			Vector3 walkerPosition = rWalker.Position;
			Owner.Position = walkerPosition;
			rWalker.Position = ourPosition;
		}

		public override void Reset()
		{
			if (!IsActive || Host.IsClient) 
				return;

			walkers.ForEach((x) => x.Delete());
		}
	}
}
