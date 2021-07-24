using guesswho.teams;
using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho.walker
{
	public partial class Walker : AnimEntity, IUse
	{
		TimeSince lastUse = 0;
		bool IUse.OnUse(Entity user)
		{
			if (user is not Player ply)
				return false;

			if (ply.Team is Hunters)
				return false;

			bool canUse = lastUse > .5f;
			lastUse = 0;

			if (!canUse)
				return false;

			Outfit walkerOutfit = new Outfit(ply, Outfit);
			Outfit playerOutfit = new Outfit(this, ply.Outfit);

			ply.Outfit.Clear();
			ply.Outfit = walkerOutfit;
			ply.Outfit.ApplyOutfit();

			this.Outfit.Clear();
			this.Outfit = playerOutfit;
			this.Outfit.ApplyOutfit();

			this.TakeDecalsFrom(ply);
			ply.RemoveAllDecals();

			return true;
		}

		bool IUse.IsUsable(Entity user)
		{
			return true;
		}
	}
}
