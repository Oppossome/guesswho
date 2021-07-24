using Sandbox;
using System;
using System.Collections.Generic;


namespace guesswho.skills
{
	class ShrinkSkill : BaseSkill
	{
		public override Texture Icon => Texture.Load("/ui/weapons/dm_pistol.png");
		public override float SkillDuration => 10;
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
			Owner.Scale = Owner.Outfit.Height - Math.Clamp(scale, 0, 0.75f);
		}
	}
}
