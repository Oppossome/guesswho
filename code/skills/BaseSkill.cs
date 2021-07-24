using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho.skills
{
	public abstract partial class BaseSkill : NetworkComponent
	{
		public abstract float SkillDuration { get; }
		public abstract string Name { get; }
		public abstract string Icon { get; }

		[Net, Predicted]
		public TimeSince SinceUsed { get; set; }
		public Player Owner;
		public bool IsActive;

		public virtual void OnUse(Player owner) 
		{
			IsActive = true;
			SinceUsed = 0;
			Owner = owner;
		}

		public virtual void OnEnd()
		{
			Owner.SkillFinished();
		}

		public virtual void Tick()
		{
			if(SinceUsed >= SkillDuration)
				OnEnd();
		}
	}
}
