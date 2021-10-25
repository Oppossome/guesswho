using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace guesswho
{
	interface IHumanoid
	{
		public Outfit Outfit { get; set; }
		public abstract string GetName();
	}
}
