﻿using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace guesswho.teams
{
	public abstract class BaseTeam : NetworkComponent
	{
		public int LivingCount => Players.Where((Player ply) => ply.LifeState == LifeState.Alive).ToList().Count;
		public List<Player> Players { get; set; } = new();
		public abstract String Name { get; }

		public void OnPlayerJoined(Player ply)
		{
			if (!Players.Contains(ply))
			{
				Log.Info($"{ply.GetClientOwner().Name} has joined team {Name}");
				Players.Add(ply);
			}
		}

		public void OnPlayerLeft(Player ply)
		{
			Log.Info($"{ply.GetClientOwner().Name} has left team {Name}");
			Players.Remove(ply);
		}

		public virtual bool ShouldPlayerMove(Player ply) => true;
		public virtual void OnPlayerRespawned(Player ply) { }
		public virtual void OnPlayerDied(Player ply) { }
	}
}