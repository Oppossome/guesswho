using guesswho.skills;
using guesswho.rounds;
using guesswho.teams;
using guesswho.ui;
using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace guesswho
{
	public partial class Game : Sandbox.Game {
		public static Game Instance => Current as Game;
		public static BaseRound CurrentRound => Instance.Round;
		
		[Net, Change] private BaseRound Round { get; set;}
		
		[Net] public Hunters Hunters { get; set;}
		
		[Net] public Hiders Hiders { get; set;}
		
		public Game()
		{
			_ = DoTicking();
			if (Host.IsServer)
			{
				new HudEntity();
				Hunters = new();
				Hiders = new();
			}
		}

		public override void PostLevelLoaded()
		{
			if (Host.IsServer)
			{
				Entity.All.OfType<Prop>().ToList().ForEach((x) => x.Delete());
				
				if (NavMesh.IsLoaded)
				{
					SetRound(new WaitingRound());
				} else
				{
					SetRound(new ErrorRound());
				}
			}
		}

		public void SetRound(BaseRound round)
		{
			BaseRound lastRound = CurrentRound;
			Round = round;

			lastRound?.OnRoundEnd();
			CurrentRound?.OnRoundStart();
		}

		BaseRound lastRound;
		private void OnRoundChanged()
		{
			lastRound?.OnRoundEnd();
			Round?.OnRoundStart();
			lastRound = Round;
		}

		public override void ClientJoined(Client cl)
		{
			base.ClientJoined(cl);

			Player nPawn = new Player();
			cl.Pawn = nPawn;

			// If the pawn doesn't get respawned treat it as dead.
			nPawn.LifeState = LifeState.Dead; 
			CurrentRound?.OnPlayerJoin(nPawn);
		}

		public override void ClientDisconnect(Client cl, NetworkDisconnectionReason reason)
		{
			Player ply = cl.Pawn as Player;
			CurrentRound?.OnPlayerLeft(ply);
			ply.Team?.OnPlayerLeft(ply);

			
			base.ClientDisconnect(cl, reason);
		}

		public override void DoPlayerNoclip(Client player)
		{
			//base.DoPlayerNoclip(player);
		}

		private async Task DoTicking()
		{
			while (!Global.IsClosing)
			{
				await Task.DelaySeconds(1);
				CurrentRound?.OnTick();
			}
		}
	}
}