using Sandbox;
using System;
using System.Collections.Generic;


namespace guesswho.animation
{
	public class CitizenAnimator
	{
		AnimEntity Owner { get; set; }

		CitizenAnimationHelper animHelper;
		Vector3 lookDir;
		float duck = 0;

		public CitizenAnimator(AnimEntity ent)
		{
			animHelper = new(ent);
			Owner = ent;
		}

		public void Tick(Vector3 wishVelocity, Vector3 velocity, bool doCrouch = false)
		{
			if ( wishVelocity.WithZ(0).Length > .5f)
			{
				Rotation targetRotation = Rotation.LookAt(wishVelocity.Normal, Vector3.Up);
				Owner.Rotation = Rotation.Lerp(Owner.Rotation, targetRotation, Time.Delta * 20);
			}

			lookDir = Vector3.Lerp(lookDir, velocity.WithZ(0) * 1000, Time.Delta * 100);
			animHelper.WithLookAt(Owner.EyePosition + lookDir);

			animHelper.WithWishVelocity(wishVelocity);
			animHelper.WithVelocity(velocity);

			
			//Is the player on the ground
			Owner.SetAnimBool("b_grounded", Owner.GroundEntity is not null);


			//Handle the duck lerping
			if (doCrouch) duck = duck.LerpTo(1, Time.Delta * 10);
			else duck = duck.LerpTo(0, Time.Delta * 5);

			Owner.SetAnimBool("b_ducked", doCrouch);
			Owner.SetAnimFloat("duck", duck);
		}
	}
}
