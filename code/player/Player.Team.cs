using guesswho.teams;
using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho
{
	public partial class Player : Sandbox.Player
	{
		[Net, OnChangedCallback] BaseTeam plyTeam { get; set; }
		BaseTeam lastTeam { get; set; }

		public BaseTeam Team {
			get => plyTeam;
			set {
				if (lastTeam == value) return;
				lastTeam?.OnPlayerLeft(this);
				
				lastTeam = value;
				plyTeam = value;

				value?.OnPlayerJoined(this);
			}
		}

		private void OnplyTeamChanged()
		{
			Team = plyTeam;
		}
	}
}
