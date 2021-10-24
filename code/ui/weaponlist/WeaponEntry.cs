using BaseWeapon = guesswho.weapons.BaseWeapon;
using guesswho.weapons;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace guesswho.ui
{
	//[UseTemplate]
	public partial class WeaponEntry : Panel
	{
		public BaseWeapon Owner { get; set; }
		public Entity Pawn { get; set; }
		public int Index { get; set; }

		Label ammo { get; set; }
		Panel icon { get; set; }

		public WeaponEntry(BaseWeapon weapon)
		{
			Owner = weapon;

			Style.BackgroundImage = Texture.Load(Owner.Icon);
			Style.BackgroundRepeat = BackgroundRepeat.NoRepeat;
			Style.BackgroundSizeX = Length.Pixels(110);
			Style.BackgroundSizeY = Length.Pixels(110);

			SetTemplate("/ui/weaponlist/WeaponEntry.html");
		}

		public override void Tick()
		{
			base.Tick();
			ammo.Text = Owner.Ammo.ToString();

			bool dState = Pawn.ActiveChild == Owner;
			SetClass("active", dState);
		}
	}
}
