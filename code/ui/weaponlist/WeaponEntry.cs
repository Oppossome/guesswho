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

			PanelBackground pBackground = new PanelBackground();
			pBackground.Repeat = BackgroundRepeat.NoRepeat;
			pBackground.SizeX = Length.Pixels(110);
			pBackground.SizeY= Length.Pixels(110);
			pBackground.Texture = Owner.Icon;

			Style.Background = pBackground;
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
