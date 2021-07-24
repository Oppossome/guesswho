using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho.weapons
{
	public partial class SMG : BaseWeapon
	{
		public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
		public override Texture Icon => Texture.Load("/ui/weapons/dm_smg.png");
		public override int SlotLocation => 2;

		public override float PrimaryRate => 15;
		public override float ReloadTime => 4f;
		public override int ClipSize => 30;

		public override void Spawn()
		{
			base.Spawn();

			SetModel("weapons/rust_smg/rust_smg.vmdl");
		}

		public override void AttackPrimary()
		{
			TimeSinceSecondaryAttack = 0;
			TimeSincePrimaryAttack = 0;

			if(Ammo == 0)
			{
				Reload();
				return;
			}

			Ammo -= 1;
			ShootEffects();
			PlaySound("rust_smg.shoot");

			ShootBullet(.05f, 1.5f, 8.0f);
		}

		[ClientRpc]
		protected override void ShootEffects()
		{
			Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");
			base.ShootEffects();
		}
	}
}
