using guesswho.teams;
using Sandbox;
using System;
using System.Collections.Generic;


namespace guesswho.skills
{
	public class SacraficeSkill : BaseSkill
	{
		public override string Name => "Sacrafice";
		public override float SkillDuration => 8;

		public override void Reset()
		{
			if (!IsActive)
				return;

			Owner.RenderColor = Color.White;
		}

		public override void OnEnd()
		{
			base.OnEnd();

			Vector3 effectPosition = Owner.Position + Vector3.Up * 32;
			Sound.FromWorld("rust_pumpshotgun.shootdouble", effectPosition);
			Particles.Create("particles/explosion", effectPosition);

			if (Host.IsClient) return;

			Owner.TakeDamage(DamageInfo.Generic(10000));

			foreach(Client client in Client.All)
			{
				if(client.Pawn is Player ply)
				{
					if (ply.LifeState != LifeState.Alive)
						continue;

					if(ply.Team is not Hunters && ply.Team is not null)
						continue;

					float dist = Vector3.DistanceBetween(Owner.Position, ply.Position);
					float damage = 150 - MathF.Pow(dist / 20, 2);
					if (damage < 0) continue;

					DamageInfo dmg = DamageInfo.Generic(damage)
						.WithAttacker(Owner);

					ply.TakeDamage(dmg);
				}
			}
		}

		public override void Tick()
		{
			if (Host.IsServer)
			{
				float TimeElapsed = .5f + MathF.Sin(MathF.Pow(SinceUsed + 1.25f, 2)) / 2;
				Owner.RenderColor = Color.Lerp(Color.Red, Color.White, TimeElapsed);
			}

			base.Tick();
		}
	}
}
