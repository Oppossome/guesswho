using guesswho.walker;
using guesswho.teams;
using guesswho.ui;
using Sandbox;
using System;
using System.Collections.Generic;


namespace guesswho.rounds
{
	public partial class ResultRound : BaseRound
	{
		public override string RoundName => "Result Round";
		public override float RoundLength => 15;

		[Net, OnChangedCallback]
		public BaseTeam Winners { get; set;}

		void OnWinnersChanged()
		{
			this.RoundPanel = new RoundPanel(this, $"{Winners.Name} Win!");
			Local.Hud.AddChild(RoundPanel);
		}

		public override void OnRoundEnd()
		{
			if(Host.IsServer)
			{
				Walker.RemoveAll();

				foreach (Client client in Client.All)
				{
					Player ply = client.Pawn as Player;
					ply.Team = null;
				}
			}

			base.OnRoundEnd();
		}

		public override void OnTick()
		{
			if (Host.IsServer)
			{
				if(TimeLeft == 0)
				{
					bool canReplay = Client.All.Count >= Settings.PlayersRequired;
					Game.Instance.SetRound(canReplay ? new HidingRound() : new WaitingRound());
					return;
				}
			}
		}
	}
}
