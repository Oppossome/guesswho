using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho.skills
{
	public partial class InvisibilitySkill : BaseSkill
	{
		public override string Name => "The Hidden";
		public override float SkillDuration => 8;

		[Net]
		float plyAlpha { get; set; } = 1;

		void SetOwnerAlpha(float alpha)
		{
			plyAlpha = alpha;
			Owner.RenderAlpha = alpha;
			foreach (ModelEntity child in Owner.Children)
				child.RenderAlpha = alpha;

		}

		public override void OnEnd()
		{
			base.OnEnd();

			if(Host.IsServer)
				SetOwnerAlpha(1);
		}

		public override void Tick()
		{
			base.Tick();

			if(Host.IsServer)
			{
				float delta = Math.Clamp(SinceUsed / SkillDuration, 0, 1);
				float scale = MathF.Sin(delta * MathF.PI) * 5;

				SetOwnerAlpha( Math.Clamp(1 - scale, 0.15f, 1) );
			}
		}

		public override void PostCamera()
		{
			if (!IsActive) return;
			
			if(Owner.RenderAlpha > plyAlpha)
				SetOwnerAlpha(plyAlpha);
		}
	}
}
