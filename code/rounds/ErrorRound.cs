using guesswho.player;
using guesswho.ui;
using Sandbox;

namespace guesswho.rounds
{
	public partial class ErrorRound : BaseRound
	{
		public override string RoundName => "ERROR";
		public override float RoundLength => -1;

		public override void OnRoundStart()
		{
			base.OnRoundStart();

			if(Host.IsClient)
			{
				RoundPanel = Local.Hud.AddChild<ErrorPanel>();
			}
		}

		public override void OnTick()
		{
			
		}
	}
}
