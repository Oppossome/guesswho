using Sandbox.UI;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace guesswho.rounds
{
	public abstract partial class BaseRound : BaseNetworkable
	{
		[Net] public TimeSince TimeElapsed { get; set; } = 0;
		public abstract float RoundLength { get; }
		public abstract string RoundName { get; }
		public Panel RoundPanel { get; set; }

		public int TimeLeft {
			get {
				if (RoundLength == -1) return (int)TimeElapsed;
				return (int)Math.Max(RoundLength - TimeElapsed, 0);
			}
		}

		public virtual void OnPlayerRespawn(Player ply)
		{
			ply.Team?.OnPlayerRespawned(ply);
		}

		public virtual void OnPlayerDied(Player ply) {
			ply.Team?.OnPlayerDied(ply);
		}

		public virtual void OnRoundEnd(){
			RoundPanel?.Delete(true);
			foreach(Player ply in Entity.All.OfType<Player>().ToList())
				ply.Skill?.Reset();
		}

		public abstract void OnTick();
		public virtual void OnRoundStart() { }
		public virtual void OnPlayerLeft(Player ply) { }
		public virtual void OnPlayerJoin(Player ply) => ply.Respawn();
		public virtual bool CanPlayerDamage(Player ply, TraceResult tr) => false;
	}
}
