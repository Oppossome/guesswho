using Sandbox;
using System;
using System.Collections.Generic;


namespace guesswho
{
	public partial class Ragdoll : ModelEntity
	{
		public TimeSince Lifetime { get; set; } = 0;

		public Ragdoll() { }
		public Ragdoll(ModelEntity ent, DamageInfo dmg)
		{
			Position = ent.Position;
			Rotation = ent.Rotation;
			MoveType = MoveType.Physics;
			
			UsePhysicsCollision = true;
			EnableAllCollisions = true;
			Scale = ent.Scale;

			SetModel("models/citizen/citizen.vmdl");
			this.CopyBonesFrom(ent);
			CopyMaterialGroup(ent);
			TakeDecalsFrom(ent);
			CopyBodyGroups(ent);

			SetInteractsAs(CollisionLayer.Debris);
			SetInteractsWith(CollisionLayer.WORLD_GEOMETRY);
			SetInteractsExclude(CollisionLayer.Player | CollisionLayer.Debris);

			foreach (ModelEntity clothing in ent.Children)
			{
				if (!clothing.Tags.Has("clothing"))
					return;

				ModelEntity nClothing = new(clothing.GetModel().Name, this);
				nClothing.CopyMaterialGroup(clothing);
			}

			if (dmg.Flags.HasFlag(DamageFlags.Bullet) || dmg.Flags.HasFlag(DamageFlags.PhysicsImpact))
			{
				PhysicsBody body = dmg.BoneIndex > 0 ? GetBonePhysicsBody(dmg.BoneIndex) : null;
				if (body != null) body.ApplyImpulseAt(dmg.Position, dmg.Force * body.Mass);
				else PhysicsGroup.ApplyImpulse(dmg.Force);
			}
		}

		[Event.Tick.Server]
		private void DoTick()
		{
			float alpha = 1 - Math.Clamp(Lifetime - 8, 0, 1);

			if (RenderAlpha == alpha)
				return;
			
			RenderAlpha = alpha;

			foreach (ModelEntity clothing in Children)
				clothing.RenderAlpha = alpha;
			
			if(alpha == 0)
				Delete();
		}
	}
}
