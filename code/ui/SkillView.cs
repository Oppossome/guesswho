using guesswho.skills;
using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace guesswho.ui
{
	public class SkillView : Panel
	{
		public string Skillname { get; set; }
		public Panel Progress { get; set;}
		Player focus;

		public SkillView()
		{
			SetTemplate("/ui/SkillView.html");
			
		}

		[Event("cam.focused")]
		void SetFocused(Entity tEnt)
		{
			focus = tEnt as Player;
			BuildView();
		}

		BaseSkill lastSkill;
		void BuildView()
		{
			BaseSkill currSkill = focus.Skill;
			lastSkill = currSkill;

			SetClass("active", currSkill is not null);
			Progress.Style.Width = null;
			Progress.Style.Dirty();

			if (currSkill is null)
			{
				Skillname = "";
				return;
			}

			Skillname = currSkill.Name;
		}

		public override void Tick()
		{
			base.Tick();

			if (focus.Skill != lastSkill)
			{
				BuildView();
				return;
			}

			SetClass("hidden", focus.SkillRechargeRate() == 0);

			float progCent = 0;
			if(lastSkill is not null)
			{
				if (lastSkill.IsActive)
				{
					progCent = lastSkill.SinceUsed / lastSkill.SkillDuration;
				}
			} else
			{
				progCent = focus.SkillRecharge / 100;
			}

			progCent = Math.Clamp(progCent, 0, 100);
			Progress.Style.Width = Length.Percent(100 * progCent);
			Progress.Style.Dirty();
		}
	}
}
