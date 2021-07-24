using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho.weapons
{
	public partial class Pistol : BaseWeapon
	{
		public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
		public override Texture Icon => Texture.Load("/ui/weapons/dm_pistol.png");
		public override int SlotLocation => 1;

		public override float PrimaryRate => 5;
		public override float ReloadTime => 2.3f;
		public override int ClipSize => 15;

		public override void Spawn()
		{
			base.Spawn();

			SetModel("weapons/rust_pistol/rust_pistol.vmdl");
		}

		public override bool CanPrimaryAttack()
		{
			return base.CanPrimaryAttack() && Input.Pressed(InputButton.Attack1);
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
			PlaySound("rust_pistol.shoot");

			ShootBullet(0.02f, 1.5f, 12);
		}
	}
}