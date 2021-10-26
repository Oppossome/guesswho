using guesswho.weapons;
using guesswho.walker;
using guesswho.player;
using guesswho.teams;
using guesswho.ui;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace guesswho.rounds
{
	class HuntingRound : BaseRound
	{
		public override string RoundName => "Hunting Round";
		public override float RoundLength => 180;

		public override void OnRoundStart()
		{
			if (Host.IsClient)
			{
				this.RoundPanel = new RoundPanel(this);
				Local.Hud.AddChild(RoundPanel);
				return;
			}

			// Stops idiots from being idiots I guess...	
			foreach(Player ply in Game.Instance.Hiders.Players)
				if (ply.Team is not null && ply.LifeState == LifeState.Dead)
					ply.Respawn();

			foreach (Player ply in Game.Instance.Hunters.Players)
			{
				// Stops idiots from being idiots I guess...	
				if (ply.Team is not null && ply.LifeState == LifeState.Dead)
					ply.Respawn();


				var pInv = ply.Inventory;
				pInv.Add(new Pistol(), true);
				pInv.Add(new Shotgun());
				pInv.Add(new SMG());
			}
		}

		public override void OnPlayerDied(Player ply)
		{
			base.OnPlayerDied(ply);
			if(ply.LastAttacker.IsValid())
				ply.LastAttacker.Health = Math.Min(100, ply.LastAttacker.Health + 40);
		}

		public override void OnPlayerJoin(Player ply)
		{
			ply.Camera = new SpectateCamera();
		}

		public override bool CanPlayerDamage(Player ply, TraceResult tr)
		{
			if (ply.Team is Hunters && tr.Entity is Walker) {
				DamageInfo wDamage = DamageInfo.Generic(10)
					.WithWeapon(ply.ActiveChild)
					.WithAttacker(tr.Entity);
					
				ply.TakeDamage(wDamage);
			}

			if (tr.Entity is Player tPly && tPly.Team is Hunters)
				return false;

			return true;
		}

		void teamWon(BaseTeam winningTeam)
		{
			ResultRound resultRound = new();
			resultRound.Winners = winningTeam;
			Game.Instance.SetRound(resultRound);
		}

		public override void OnTick()
		{
			if (Host.IsServer)
			{
				if (Client.All.Count < Settings.PlayersRequired)
				{
					Game.Instance.SetRound(new WaitingRound());
					return;
				}

				if (Game.Instance.Hunters.LivingCount == 0)
				{
					teamWon(Game.Instance.Hiders);
					return;
				}

				if (Game.Instance.Hiders.LivingCount == 0)
				{
					teamWon(Game.Instance.Hunters);
					return;
				}

				if (TimeLeft == 0)
					teamWon(Game.Instance.Hiders);
			}
		}
	}
}
