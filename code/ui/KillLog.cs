using BaseWeapon = guesswho.weapons.BaseWeapon;
using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace guesswho.ui
{
	class KillEntry : Panel
	{
		TimeSince passed { get; set; }
		Label attacker { get; set; }
		Label victim { get; set; }
		Image weapon { get; set;}

		public KillEntry(string attacker, string icon, string victim)
		{
			SetTemplate("/ui/KillLog.html");
			weapon.Texture = Texture.Load(icon);
			this.attacker.Text = attacker;
			this.victim.Text = victim;
			passed = 0;
		}

		public override void Tick()
		{
			SetClass("active", passed > .1f && passed < 2f);
			if (passed > 2.5) Delete();
			base.Tick();
		}
	}

	public partial class KillLog : Panel
	{
		public static KillLog Instance;

		public KillLog()
		{
			StyleSheet.Load("/ui/KillLog.scss");
			Instance = this;
		}

		[ClientRpc]
		public static void AddClientEntry(string attacker, string victim, string icon)
		{
			KillEntry nEntry = new(attacker, icon, victim);
			Instance.AddChild(nEntry);
		}

		public static void AddEntry(DamageInfo dInfo, Entity victim)
		{
			if (dInfo.Attacker is not INamed aName)
				return;

			if (victim is not INamed vName)
				return;

			if (dInfo.Weapon is BaseWeapon wep) AddClientEntry(To.Everyone, aName.GetName(), vName.GetName(), wep.Icon);
			else AddClientEntry(To.Everyone, aName.GetName(), vName.GetName(), "/ui/dead.png");
		}
	}
}
