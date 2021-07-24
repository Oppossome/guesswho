using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace guesswho.walker.tasks
{
	public abstract class BaseTask
	{
		public abstract float Speed { get; }
		public abstract string Name { get; }

		public bool Completed { get; set; } = false;
		public Walker Owner { get; set; }


		public BaseTask(Walker owner) => Owner = owner;
		public abstract Vector3 CalculateInputVelocity();
	}
}
