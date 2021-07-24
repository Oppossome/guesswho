using guesswho.animation;
using guesswho.walker.tasks;
using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace guesswho.walker
{
	public partial class Walker : AnimEntity
	{
		public Outfit Outfit { get; set;}

		CitizenAnimator animator { get; set;}

		public override void Spawn()
		{
			base.Spawn();

			SetModel("models/citizen/citizen.vmdl");
			animator = new(this);

			CollisionGroup = CollisionGroup.Player;
			EyePos = Position + Vector3.Up * 64;

			SetupPhysicsFromCapsule(PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius(72, 8));
			MoveType = MoveType.MOVETYPE_WALK;
			EnableHitboxes = true;

			Outfit = new Outfit(this);
			Outfit.ApplyOutfit();

			Health = 100;
		}

		public BaseTask CurrentTask;
		Vector3 inputVelocity;

		[Event.Tick.Server]
		public void Tick()
		{
			if (CurrentTask is not null && !CurrentTask.Completed)
			{
				inputVelocity = CurrentTask.CalculateInputVelocity();
				Velocity = Velocity.AddClamped(inputVelocity * Time.Delta * 500, CurrentTask.Speed);
			}
			else
			{
				int chance = Rand.Int(6);

				CurrentTask = chance != 1 ? 
					new WalkTask(this) :
					new RunTask(this);
			}

			Move(Time.Delta);

			animator.Tick(inputVelocity, Velocity);
		}

		protected virtual void Move(float timeDelta)
		{
			BBox bbox = BBox.FromHeightAndRadius(64, 4);
			MoveHelper move = new MoveHelper(Position, Velocity);
			move.Trace = move.Trace.Ignore(this).Size(bbox.Mins, bbox.Maxs);
			move.MaxStandableAngle = 65;
	
			move.TryUnstuck();
			move.TryMoveWithStep(timeDelta, 20);
		
			TraceResult tr = move.TraceDirection(Vector3.Down * 10);

			if (move.IsFloor(tr))
			{
				GroundEntity = tr.Entity;

				if (!tr.StartedSolid) // Keeps the entity on the ground
					move.Position = tr.EndPos;
				
				if(inputVelocity.Length > .5f)
				{
					Vector3 movement = move.Velocity.Dot(inputVelocity.Normal);
					move.Velocity -= movement * inputVelocity.Normal;
					move.ApplyFriction(tr.Surface.Friction * 10, timeDelta);
					move.Velocity += movement * inputVelocity.Normal;
				}else
				{
					move.ApplyFriction(tr.Surface.Friction * 10, timeDelta);
				}
			}
			else
			{
				move.Velocity += PhysicsWorld.Gravity * timeDelta;
				GroundEntity = null;
			}

			Position = move.Position;
			Velocity = move.Velocity;
		}

		public override void OnAnimEventFootstep(Vector3 position, int foot, float volume)
		{
			var tr = Trace.Ray(position, position + Vector3.Down * 20)
				.Radius(1)
				.Ignore(this)
				.Run();

			if (!tr.Hit || Host.IsServer) return;

			float stepDistance = Vector3.DistanceBetween(Input.Position, tr.EndPos);
			float stepVolume = (float)Math.Pow(1 - (stepDistance / 512), .5);

			if (stepVolume < 0)
				return;

			tr.Surface.DoFootstep(this, tr, foot, stepVolume);
		}

		DamageInfo lastDamage;
		public override void OnKilled()
		{
			DeleteAsync(5f);
			base.OnKilled();
			LifeState = LifeState.Dead;
			EnableAllCollisions = false;
			EnableDrawing = false;

			Ragdoll wRagdoll = new(this, lastDamage);
		}

		public override void TakeDamage(DamageInfo info)
		{
			lastDamage = info;
			base.TakeDamage(info);
			this.ProceduralHitReaction(info);
		}

		public static void SpawnWalkers(int count = 1)
		{
			List<SpawnPoint> spawnPoints = Entity.All.OfType<SpawnPoint>().ToList();

			for (int i = 0; i < count; i++)
			{
				Vector3 randomSpawnPos = spawnPoints[Rand.Int(0, spawnPoints.Count - 1)].Position;
				Vector3? potentialSpawn = NavMesh.GetPointWithinRadius(randomSpawnPos, 500, 3500);

				if (potentialSpawn is not Vector3 spawnLocation)
					return;

				Walker walker = new Walker();
				walker.Position = spawnLocation;
			}
		}

		public static void RemoveAll()
		{
			Entity.All.OfType<Walker>().ToList()
				.ForEach(walker => walker.Delete());
		}
	}
}
