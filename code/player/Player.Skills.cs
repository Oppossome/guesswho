using guesswho.skills;
using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho
{
	public partial class Player : Sandbox.Player
	{		
		[Net]
		public BaseSkill Skill { get; set; }

		[Net]
		public float SkillRecharge { get; set;}

		public void SkillUse()
		{
			Host.AssertServer();
			Skill.OnUse(this);
		}

		public float SkillRechargeRate()
		{
			if(LifeState == LifeState.Alive)
			{
				if(Team is not null)
					return Team.SkillRechargeRate( this );

				return 10;
			}


			return 0;
		}

		public void SkillTick()
		{
			float rechargeRate = SkillRechargeRate();

			if (rechargeRate == 0)
				return;

			if (Skill is not null) { 
				if(Skill.IsActive)
				{
					Skill.Tick();
					return;
				}
			} else if(Host.IsServer)
			{
				SkillRecharge += rechargeRate * Time.Delta;

				if(SkillRecharge >= 100)
				{
					Skill = new ShrinkSkill();
					SkillRecharge = 0;
				}
			}

			if (Input.Pressed(InputButton.Menu))
				Skill?.OnUse(this);
		}
	}
}
