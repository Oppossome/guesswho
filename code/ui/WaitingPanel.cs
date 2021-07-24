using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;
using System.Collections.Generic;

namespace guesswho.ui
{
	public class WaitingPanel : Panel
	{
		public string HudText { get; set; } = "Waiting for players";

		List<Panel> playerPanels = new();
		Panel playerPanel { get; set; }

		public WaitingPanel()
		{
			SetTemplate("/ui/WaitingPanel.html");
			AddClass("status");
		}


		public void SetPlayerCounts(int online, int needed)
		{
			playerPanels.ForEach(x => x.Delete());
			playerPanels = new();

			for (int i = 1; i <= needed; i++)
			{
				Panel plyPanel = playerPanel.Add.Panel("segment");
				plyPanel.SetClass("active", i <= online);
				playerPanels.Add(plyPanel);
			}
		}	
	}
}
