using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho
{
	class Camera : Sandbox.Camera
	{
		const float MaxZoomOut = 100;
		const float AlphaStart = 45;
		const float AlphaEnd = 16;

		public float Alpha = 0;

		float wishZoom = 0;

		public Camera()
		{
			FieldOfView = 80;
		}

		public override void Activated()
		{
			Event.Run("cam.focused", Local.Pawn);
		}

		public override void Update()
		{
			Player localPly = Local.Pawn as Player;

			TraceResult tr = Trace.Ray(localPly.EyePos, localPly.EyePos + localPly.EyeRot.Forward * -wishZoom)
				.Ignore(localPly)
				.Size(2)
				.Run();

			Rot = localPly.EyeRot;
			Pos = tr.EndPos;

			float camDist = Vector3.DistanceBetween(localPly.EyePos, tr.EndPos);
			Alpha = Math.Clamp((camDist - AlphaEnd) / (AlphaStart - AlphaEnd), 0, 1);
			Viewer = (camDist < AlphaEnd ? localPly : null);

			localPly.RenderAlpha = Alpha;
			foreach (ModelEntity ent in localPly.Children)
				ent.RenderAlpha = Alpha;

			localPly.Skill?.PostCamera();
		}

		public override void BuildInput(InputBuilder input)
		{
			base.BuildInput(input);
			Player localPly = Local.Pawn as Player;

			if (input.Pressed(InputButton.View))
				wishZoom = (wishZoom == MaxZoomOut ? 0 : MaxZoomOut);

			if (localPly.ActiveChild is null)
				wishZoom = Math.Clamp(wishZoom - 15 * input.MouseWheel, 0, MaxZoomOut);
		}
	}
}
