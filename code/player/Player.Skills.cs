using guesswho.skills;
using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho
{
	public partial class Player : Sandbox.Player
	{
		[Net]
		public List<int> Skills { get; set; } = new();
		
		[Net, OnChangedCallback]
		public BaseSkill ActiveSkill { get; set; }

		public void SkillUse()
		{
			Host.AssertServer();

			if (Skills.Count == 0)
				return;

			SkillEntry skill = SkillRegistry.Skills[0];
			Skills.RemoveAt(0);

			ActiveSkill = skill.Create();
			ActiveSkill.OnUse(this);
		}

		public void SkillFinished()
		{
			ActiveSkill = null;
		}

		void OnActiveSkillChanged()
		{
			if (ActiveSkill is null) return;
			ActiveSkill.OnUse(this);
		}

		public void SkillTick()
		{
			if(ActiveSkill is not null)
			{
				ActiveSkill.Tick();
				return;
			}

			if (Input.Pressed(InputButton.Menu) && Host.IsServer)
				SkillUse();
		}
	}
}
