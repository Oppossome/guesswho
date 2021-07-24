using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace guesswho.ui
{
	class HudEntity : Sandbox.HudEntity<RootPanel>
	{
		public HudEntity()
		{
			if (!Host.IsClient)
				return;

			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<Hud>();
		}
	}
}
