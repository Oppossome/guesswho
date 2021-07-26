using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho.skills
{
	public abstract partial class BaseSkill : NetworkComponent
	{
		public abstract float SkillDuration { get; }
		public abstract string Name { get; }
		public Player Owner;

		[Net, Predicted]
		public TimeSince SinceUsed { get; set; }

		[Net, Predicted]
		public bool IsActive { get; set; }

		public virtual void OnUse(Player owner) 
		{
			IsActive = true;
			SinceUsed = 0;
			Owner = owner;
		}

		public virtual void OnEnd()
		{
			Owner.Skill = null;
			Reset();
		}

		public virtual void Tick()
		{
			if(SinceUsed >= SkillDuration)
				OnEnd();
		}

		public virtual void Reset() => Owner.Skill = null;
		public virtual void PostCamera() { }

		public static BaseSkill RandomSkill()
		{
			switch(Rand.Int(1, 3))
			{
				case 1:
					return new ShrinkSkill();
				case 2:
					return new InvisibilitySkill();
				case 3:
					return new CrowdSkill();
				case 4:
					return new SwapSkill();
				case 5:
					return new SacraficeSkill();
				default:
					return new ShrinkSkill();
			}
		}
	}
}
