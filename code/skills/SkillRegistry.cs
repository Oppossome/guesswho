using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace guesswho.skills
{
	public struct SkillEntry
	{
		public Func<BaseSkill> Create { get; set;} 
		public string Name { get; set; }
		public string Icon { get; set; }
		public int Id { get; set;}

		public static void FromSkill(BaseSkill skill, Func<BaseSkill> creator)
		{
			SkillEntry nEntry = new();
			nEntry.Id = SkillRegistry.Skills.Count;
			nEntry.Name = skill.Name;
			nEntry.Icon = skill.Icon;
			nEntry.Create = creator;

			SkillRegistry.Skills.Add(nEntry);
		}
	}

	public static class SkillRegistry
	{
		public static List<SkillEntry> Skills = new();

		public static void RegisterSkills()
		{
			SkillEntry.FromSkill(new ShrinkSkill(), () => new ShrinkSkill());
		}
	}
}
