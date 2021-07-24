using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace guesswho.ui
{
	[UseTemplate("/ui/Health.html")]
	class HealthSegment : Panel
	{
		Panel health { get; set; }
		Panel linger { get; set; }

		private void updatePanel(Panel toUpdate, float amnt, Color? clr = null)
		{
			float percentage = Math.Clamp(amnt, 0, 10) * 10;
			toUpdate.Style.Display = (percentage < 3 ? DisplayMode.None : DisplayMode.Flex);
			toUpdate.Style.Width = Length.Percent(percentage);
			
			if(clr is not null)
				toUpdate.Style.BackgroundColor = clr;
			
			toUpdate.Style.Dirty();
		}

		public void SetProgress(float hAmnt, float lAmnt, Color clr)
		{
			updatePanel(health, hAmnt, clr);
			updatePanel(linger, lAmnt);
		}
	}

	class Health : Panel
	{
		List<HealthSegment> segments = new List<HealthSegment>();
		float linger = 0;
		Entity focus;

		public Health()
		{
			StyleSheet.Load("/ui/Health.scss");

			for(int i = 0; i < 10; i++)
				segments.Add(AddChild<HealthSegment>());
		}

		Color colorHealthy = Color.FromBytes(255, 255, 255);
		Color colorHurt = Color.FromBytes(198, 40, 40);

		[Event("cam.focused")]
		public void SetFocused(Entity tEnt)
		{
			linger = tEnt.Health;
			focus = tEnt;
		}

		public override void Tick()
		{
			float health = 0; //100 - (Time.Now * 25 % 100);

			if(focus is Player ply)
			{
				health = ply.Health;

				if (health > linger) linger = health;
				else linger = linger.LerpTo(health, Time.Delta);
			}

			float desiredState = Math.Clamp((health - 10) / 30, 0, 1);
			Color hColor = Color.Lerp(colorHurt, colorHealthy, desiredState);

			for(int i = 0; i < 10; i++)
			{
				float offset = i * 10;
				segments[i].SetProgress(health - offset, linger - offset, hColor);
			}
		}
	}
}
