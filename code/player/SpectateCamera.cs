using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace guesswho.player
{
	public class SpectateCamera : Sandbox.Camera
	{
		Player spectating { get; set; }
		bool isZoomed = true;

		public SpectateCamera()
		{
			FieldOfView = 80;
		}

		public Player Spectating
		{
			get => spectating;
			set {
				setAlpha(spectating, 1);
				Viewer = null;

				spectating = value;
				Pos = spectating.EyePos;
				Rot = spectating.EyeRot;
				Event.Run("cam.focused", spectating);
			}
		}

		public override void Deactivated()
		{
			setAlpha(spectating, 1);
			base.Deactivated();
		}

		void setAlpha(ModelEntity ent, float alpha)
		{
			if (ent is null || !ent.IsValid())
				return;

			ent.RenderAlpha = alpha;
			foreach (ModelEntity child in ent.Children)
				child.RenderAlpha = alpha;
		}

		public override void Update()
		{
			if(spectating is null || !spectating.IsValid() || spectating.LifeState != LifeState.Alive)
			{
				List<Player> livingPlayers = Entity.All.OfType<Player>()
					.Where(x => x.LifeState == LifeState.Alive).ToList();

				if (livingPlayers.Count == 0)
					return;

				Spectating = livingPlayers[0];
			}

			float camDistance = isZoomed ? 100 : 0;
			TraceResult tr = Trace.Ray(spectating.EyePos, spectating.EyePos + spectating.EyeRot.Forward * -camDistance)
				.Ignore(spectating.ActiveChild)
				.Ignore(spectating)
				.Size(2)
				.Run();

			Pos = Pos.LerpTo(tr.EndPos, Time.Delta * 10);
			Rot = Rotation.Lerp(Rot, spectating.EyeRot, Time.Delta * 10);

			float camDist = Vector3.DistanceBetween(spectating.EyePos, Pos);
			float alpha = Math.Clamp((camDist - 45) / 26, 0, 1);
			Viewer = (camDist < 16 ? spectating : null);
			setAlpha(spectating, alpha);

			spectating.Skill?.PostCamera();
		}

		public override void BuildInput(InputBuilder input)
		{
			base.BuildInput(input);
			int mDirection = 0;

			if(input.Pressed(InputButton.Attack1)) mDirection = 1;
			if(input.Pressed(InputButton.Attack2)) mDirection = -1;
			
			if(mDirection != 0)
			{
				List<Player> livingPlayers = Entity.All.OfType<Player>()
						.Where(x => x.LifeState == LifeState.Alive).ToList();

				if (livingPlayers.Count == 0)
					return;

				int targetSlot = (livingPlayers.IndexOf(spectating) + mDirection) % livingPlayers.Count;
				if (targetSlot == -1) targetSlot = livingPlayers.Count - 1;
				Spectating = livingPlayers[targetSlot];
			}

			if (input.Pressed(InputButton.View))
				isZoomed = !isZoomed;
		}
	}
}
