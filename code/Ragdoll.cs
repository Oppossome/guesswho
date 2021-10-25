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

			if(ent is IHumanoid humanoid && humanoid.Outfit != null)
			{
				Outfit outfit = new(this, humanoid.Outfit);
				outfit.ApplyOutfit();
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

			if (RenderColor.a == alpha)
				return;
			
			this.RenderColor = this.RenderColor.WithAlpha(alpha);

			foreach (ModelEntity clothing in Children)
				clothing.RenderColor = clothing.RenderColor.WithAlpha(alpha);
			
			if (alpha == 0)
				Delete();
		}
	}
}
