using guesswho.rounds;
using guesswho.teams;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;
using System.Collections.Generic;

namespace guesswho.ui
{
	public class RoundPanel : Panel
	{
		class PlayerEntry : Panel
		{
			public Player Owner { get; set; }
			Panel iconPanel;

			public PlayerEntry(Player owner)
			{
				Owner = owner;
			}

			public override void Tick()
			{
				base.Tick();

				if (!Owner.IsValid())
					return;

				if(iconPanel is null)
				{
					if(Owner.Client is not null)
					{
						Add.Image($"avatar:{Owner.Client.SteamId}", "avatar");
						iconPanel = Add.Panel("icon");
					}

					return;
				}

				iconPanel.SetClass("active", Owner.LifeState == LifeState.Dead);
				iconPanel.SetClass("focused", Game.Focused == Owner);

				float healthLerp = 1 - (Owner.Health / 100);
				iconPanel.Style.BackdropFilterBrightness = Length.Pixels(.5f + (1 - healthLerp) / 2);
				iconPanel.Style.BackdropFilterSaturate = Length.Pixels(1 - healthLerp);
				iconPanel.Style.BackdropFilterBlur = Length.Pixels(2 * healthLerp);
				iconPanel.Style.Dirty();
			}
		}

		public string RoundName { get; set; }
		public BaseRound CurrentRound;

		List<PlayerEntry> playerEntries = new();
		Panel hunterTeam { get; set; }
		Panel hiderTeam { get; set; }
		Panel timerLine { get; set; }

		public RoundPanel(BaseRound currentRound, string roundName = null)
		{
			SetTemplate("/ui/RoundPanel.html");
			AddClass("status");
			
			CurrentRound = currentRound;
			RoundName = roundName is not null ? roundName : CurrentRound.RoundName;
		}
		
		void ListPlayers()
		{
			playerEntries.ForEach(x => x.Delete());
			playerEntries = new();

			List<Player> players = Entity.All.OfType<Player>().Where( x => x.Team is not null).ToList();
			foreach (Player ply in players)
			{
				Panel tPanel = ply.Team is Hunters ? hunterTeam : hiderTeam;
				PlayerEntry pEntry = new PlayerEntry(ply);
				tPanel.AddChild(pEntry);

				playerEntries.Add(pEntry);
			}
		}

		int playerCount;
		public override void Tick()
		{
			base.Tick();

			float roundProgress = Math.Min(CurrentRound.TimeElapsed / CurrentRound.RoundLength, 1);
			timerLine.Style.Width = Length.Percent(roundProgress * 100);
			timerLine.Style.Dirty();

			List<Client> clients = Client.All.Where(x => (x.Pawn as Player).Team is not null).ToList();
			if (clients.Count != playerCount)
			{
				playerCount = clients.Count;
				ListPlayers();
			}
		}

		[ServerCmd]
		public static void SetHealth(int health)
		{
			ConsoleSystem.Caller.Pawn.Health = health;
		}
	}
}
