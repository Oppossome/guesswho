using guesswho.weapons;
using guesswho.walker;
using guesswho.ui;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace guesswho.rounds
{
	public partial class WaitingRound : BaseRound
	{
		[Net] int playersRequired { get; set; }
		public override string RoundName => "Waiting Round";
		public override float RoundLength => -1;

		public override void OnRoundStart()
		{
			if (Host.IsClient)
			{
				RoundPanel = Local.Hud.AddChild<WaitingPanel>();
				return;
			}

			if (Host.IsServer)
			{
				playersRequired = Settings.PlayersRequired;

				foreach(Player pl in Client.All.Select(x => x.Pawn as Player))
				{
					pl.Team = null;
					pl.Respawn();
				}
			}
		}

		public override void OnRoundEnd()
		{
			base.OnRoundEnd();

			if (Host.IsClient) return;
			Walker.RemoveAll();
		}

		public override bool CanPlayerDamage(Player ply, TraceResult tr)
		{	
			return true;
		}

		public override void OnPlayerRespawn(Player ply)
		{
			var plyInventory = ply.Inventory;
			plyInventory.Add(new Pistol(), true);
			plyInventory.Add(new Shotgun());
			plyInventory.Add(new SMG());
			
			base.OnPlayerRespawn(ply);
		}

		public override void OnPlayerDied(Player ply)
		{
			ply.Camera = new SpectateRagdollCamera();
		}

		int onlineCount = 0;
		public override void OnTick()
		{
			List<Player> players = Client.All.Select(x => x.Pawn as Player).ToList();

			if (Host.IsClient)
			{
				WaitingPanel waitingPanel = RoundPanel as WaitingPanel;

				if(onlineCount != players.Count)
				{
					waitingPanel.SetPlayerCounts(players.Count, playersRequired);
					onlineCount = players.Count;
				}
				
				return;
			}
			
			if(players.Count >= Settings.PlayersRequired)
			{
				Game.Instance.SetRound(new HidingRound());
			}

			List<Walker> walkers = Entity.All.OfType<Walker>().ToList();

			if(walkers.Count < 15 && players.Count != 0)
				Walker.SpawnWalkers(15 - walkers.Count);

			foreach(Client cl in Client.All)
			{
				Player ply = cl.Pawn as Player;
				if (ply.LifeState == LifeState.Dead && ply.TimeSinceDied > 5)
					ply.Respawn();
			}
		}
	}
}
