using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace guesswho.ui
{
	//[UseTemplate]
	class Hud : Panel
	{
		public Hud()
		{
			StyleSheet.Load("/ui/Hud.scss");
			SetTemplate("/ui/Hud.html");
		}


	}
}
