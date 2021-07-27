using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho.weapons
{
	public partial class Shotgun : BaseWeapon
	{
		public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
		public override string Icon => "/ui/weapons/dm_shotgun.png";
		public override int SlotLocation => 3;

		public override float ReloadTime => .5f;
		public override float PrimaryRate => 1;
		public override int ClipSize => 8;

		public override void Spawn()
		{
			base.Spawn();

			SetModel("weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl");
		}

		public override bool CanPrimaryAttack()
		{
			return base.CanPrimaryAttack() && Input.Pressed(InputButton.Attack1);
		}

		public override void AttackPrimary()
		{
			TimeSinceSecondaryAttack = 0;
			TimeSincePrimaryAttack = 0;

			if (Ammo < 1)
			{
				Reload();
				return;
			}

			Ammo -= 1;
			ShootEffects();
			PlaySound("rust_pumpshotgun.shoot");
			(Owner as AnimEntity).SetAnimBool("b_attack", true);

			ShootBullet(.15f, 0.3f, 10, 10);
		}

		public override bool CanSecondaryAttack()
		{
			return base.CanSecondaryAttack() && Input.Pressed(InputButton.Attack2);
		}

		public override void AttackSecondary()
		{
			TimeSinceSecondaryAttack = -0.5f;
			TimeSincePrimaryAttack = -0.5f;

			if(Ammo < 2)
			{
				AttackPrimary();
				return;
			}

			Ammo -= 2;
			ShootEffects();
			PlaySound("rust_pumpshotgun.shootdouble");
			(Owner as AnimEntity).SetAnimBool("b_attack", true);

			ShootBullet(.4f, 0.3f, 10, 20);
		}

		public override void OnReloadFinished()
		{
			Ammo++;
			IsReloading = false;
			if (Ammo < ClipSize) Reload();
			else FinishReload();
		}

		[ClientRpc]
		protected virtual void FinishReload()
		{
			ViewModelEntity?.SetAnimBool("reload_finished", true);
		}

		[ClientRpc]
		protected override void ShootEffects()
		{
			Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");
			ViewModelEntity?.SetAnimBool("fire", true);
			base.ShootEffects();
		}

		[ClientRpc]
		protected virtual void DoubleShootEffects()
		{
			Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");
			ViewModelEntity?.SetAnimBool("fire_double", true);
			base.ShootEffects();

			if (IsLocalPawn)
				new Sandbox.ScreenShake.Perlin(3.0f, 3.0f, 3.0f);
		}

		public override void SimulateAnimator(PawnAnimator anim)
		{
			anim.SetParam("holdtype", 3);
			anim.SetParam("aimat_weight", 1.0f);
		}
	}
}