using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace guesswho.ui
{
	class Cursor : Panel
	{
		public Cursor()
		{
			StyleSheet.Load("/ui/Cursor.scss");
		}

		public override void Tick()
		{
			base.Tick();

			if (Local.Pawn is not Player ply)
				return;

			if (ply.Camera is not Camera cam) Style.Opacity = 0;
			else Style.Opacity = 1 - cam.Alpha;
			Style.Dirty();
		}
	}
}
