using BaseWeapon = guesswho.weapons.BaseWeapon;
using guesswho.weapons;
using guesswho.player;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;


namespace guesswho.ui
{
	public partial class WeaponList : Panel
	{
		List<WeaponEntry> weaponEntries = new();
		Entity focus;

		public WeaponList()
		{
			StyleSheet.Load("/ui/weaponlist/WeaponList.scss");
			this.SetClass("hidden", true);
		}

		[Event("cam.focused")]
		public void SetFocused(Entity tEnt)
		{
			focus = tEnt;
			BuildEntries();
		}

		private void BuildEntries()
		{
			weaponEntries.ForEach((x) => x.Delete(true));
			weaponEntries.Clear();

			var wepList = focus.Children.OfType<BaseWeapon>();
			this.SetClass("hidden", wepList.Count() == 0);

			wepList.OrderBy(x => x.SlotLocation).ToList().ForEach(wep =>
			{
				WeaponEntry wEntry = new WeaponEntry(wep);
				wEntry.Index = weaponEntries.Count;
				wEntry.Pawn = focus;

				weaponEntries.Add(wEntry);
				AddChild(wEntry);
			});
		}

		public override void Tick()
		{
			base.Tick();

			bool performRebuild = true;
			Entity active = focus.ActiveChild;
			foreach(WeaponEntry w in weaponEntries)
			{
				if (!w.Owner.IsValid())
				{
					BuildEntries();
					return;
				}

				if(active is not null && w.Owner == active)
				{
					performRebuild = false;
				}
			}

			if (performRebuild)
				BuildEntries();
		}

		InputButton[] btns = new InputButton[]
		{
			InputButton.Slot1,
			InputButton.Slot2,
			InputButton.Slot3
		};

		[Event.BuildInput]
		public void ProcessClientInput(InputBuilder input)
		{
			if (this.HasClass("hidden"))
				return;

			if (focus != Local.Pawn)
				return;

			int wheelMove = input.MouseWheel;
			BaseWeapon desiredWeapon = null;

			if (wheelMove != 0)
			{
				int dIndex = (weaponEntries.FindIndex(x => x.Owner == Local.Pawn.ActiveChild) + wheelMove) % 3;
				if (dIndex == -1) dIndex = 2;
				if (dIndex <= -2) return;

				desiredWeapon = weaponEntries[dIndex].Owner;
			} else
			{
				int wantsWeapon = -1;

				for (int i = 0; i < btns.Length; i++)
				{
					if (input.Pressed(btns[i]))
					{
						wantsWeapon = i;
						break;
					}
				}

				if (wantsWeapon == -1) return;

				desiredWeapon = weaponEntries[wantsWeapon].Owner;
			}

			if(desiredWeapon is not null)
				SetActive( desiredWeapon.NetworkIdent );
		}

		[ServerCmd]
		public static void SetActive(int ident)
		{
			if (ConsoleSystem.Caller.Pawn is not Player ply)
				return;

			Entity wep = ply.Children.Where(x => x.NetworkIdent == ident).First();
			ply.ActiveChild = wep;
		}
	}
}
