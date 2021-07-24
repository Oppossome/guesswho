using Sandbox;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace guesswho.weapons
{
	public partial class BaseWeapon : Sandbox.BaseWeapon
	{
		public virtual Texture Icon { get; set; }
		public virtual int SlotLocation => -1;

		public virtual float ReloadTime => 3;
		public virtual int ClipSize => 16;
		
		[Net, Predicted] 
		public float Ammo { get; set; }

		[Net, Predicted]
		public TimeSince ReloadStarted { get; set; }

		[Net, Predicted]
		public bool IsReloading { get; set; }

		[Net, Predicted]
		public TimeSince TimeSinceEquipped { get; set; }

		public override void Spawn()
		{
			base.Spawn();
			Ammo = ClipSize;
		}

		public override void Reload()
		{
			if (IsReloading || Ammo >= ClipSize)
				return;

			IsReloading = true;
			ReloadStarted = 0;
			ReloadEffect();
		}

		[ClientRpc]
		public virtual void ReloadEffect()
		{
			(Owner as AnimEntity).SetAnimBool("b_reload", true);
			ViewModelEntity?.SetAnimBool("reload", true);
		}

		public override void Simulate(Client player)
		{
			if (TimeSinceEquipped < .5f)
				return;

			if(!IsReloading) 
				base.Simulate(player);
			else if(ReloadStarted >= ReloadTime)
				OnReloadFinished();	
		}

		public virtual void OnReloadFinished()
		{
			IsReloading = false;
			Ammo = ClipSize;
		}

		private Vector3 rVec(Random cRand)
		{
			return new Vector3(
				1 - (float)cRand.NextDouble() * 2,
				1 - (float)cRand.NextDouble() * 2,
				1 - (float)cRand.NextDouble() * 2);
		}

		public virtual void ShootBullet( float spread, float force, float damage, int count = 1)
		{
			Random rand = new Random(this.NetworkIdent + (int)(Time.Now * 60));

			for(int i = 0; i < count; i++)
			{
				Vector3 forward = Owner.EyeRot.Forward;
				forward += (rVec(rand) + rVec(rand) + rVec(rand) + rVec(rand)) * spread * .25f;
				forward = forward.Normal;

				if (!Prediction.FirstTime)
					continue;

				foreach (TraceResult tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * 5000))
				{
					if (!Game.CurrentRound.CanPlayerDamage(Owner as Player, tr))
						continue;

					if (tr.Hit)
						tr.Surface.DoBulletImpact(tr);

					if (!IsServer || !tr.Entity.IsValid())
						continue;

					float lerp = (tr.Direction * -1).Dot(tr.Normal);
					Vector3 direction = Vector3.Lerp(tr.Direction, -tr.Normal, 1 - lerp);
					DamageInfo dInfo = DamageInfo.FromBullet(tr.EndPos, direction * force * 100, damage)
						.WithFlag(DamageFlags.Bullet)
						.UsingTraceResult(tr)
						.WithAttacker(Owner)
						.WithWeapon(this);

					tr.Entity.TakeDamage(dInfo);
				}
			}
		}

		public override void ActiveStart(Entity ent)
		{
			base.ActiveStart(ent);
			TimeSinceEquipped = 0;
			IsReloading = false;
		}

		[ClientRpc]
		protected virtual void ShootEffects()
		{
			Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
			ViewModelEntity?.SetAnimBool("fire", true);
		}
	}
}
