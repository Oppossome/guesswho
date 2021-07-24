using guesswho.weapons;
using guesswho.player;
using guesswho.walker;
using guesswho.ui;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace guesswho.rounds
{
	class HidingRound : BaseRound
	{
		public override string RoundName => "Hiding Round";
		public override float RoundLength => 15;

		public override void OnRoundStart()
		{
			if (Host.IsClient)
			{
				this.RoundPanel = new RoundPanel(this);
				Local.Hud.AddChild(RoundPanel);
				return;
			}

			List<Player> players = Client.All.Select(x => x.Pawn as Player)
				.OrderBy(x => Rand.Float(0, 1)).ToList();

			int hunterCount = Math.Max(1, players.Count / Settings.HunterPerHider); 
			for(int i = 0; i < players.Count; i++)
			{
				Player cPly = players[i];
				cPly.Team = i < hunterCount ? Game.Instance.Hunters : Game.Instance.Hiders;
				cPly.Respawn();
			}

			Walker.SpawnWalkers((players.Count - hunterCount) * Settings.BotsPerHider);
		}

		public override void OnPlayerJoin(Player ply)
		{
			ply.Camera = new SpectateCamera();
		}

		public override void OnTick()
		{
			if (Host.IsServer)
			{
				if(Client.All.Count < Settings.PlayersRequired)
				{
					Game.Instance.SetRound(new WaitingRound());
					return;
				}

				if(Game.Instance.Hiders.LivingCount == 0 || Game.Instance.Hunters.LivingCount == 0)
				{
					Game.Instance.SetRound(new WaitingRound());
					return;
				}

				if(TimeLeft == 0)
				{
					Game.Instance.SetRound(new HuntingRound());
				}
			}
		}
	}
}
