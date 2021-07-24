using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace guesswho.walker.tasks
{
	public class RunTask : WalkTask
	{
		public override string Name => "Running";
		public override float Speed => 320;

		public RunTask(Walker owner) : base(owner)
		{

		}
	}
}
