using Sandbox;
using System;
using System.Collections.Generic;


namespace guesswho.skills
{
	class ShrinkSkill : BaseSkill
	{
		public override float SkillDuration => 8;
		public override string Name => "Shrink";

		public override void OnEnd()
		{
			base.OnEnd();

			if (Host.IsServer)
				Owner.Scale = Owner.Outfit.Height;
		}

		public override void Tick()
		{
			base.Tick();

			if (Host.IsClient)
				return;

			float delta = Math.Clamp(SinceUsed / SkillDuration, 0, 1);
			float scale = MathF.Sin(delta * MathF.PI) * 5;
			float height = Owner.Outfit.Height;	
			Owner.Scale = Math.Clamp(height - scale, 0.55f, height);
		}
	}
}
