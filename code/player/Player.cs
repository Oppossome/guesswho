using guesswho.animation;
using guesswho.weapons;
using guesswho.skills;
using guesswho.player;
using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho
{
	public partial class Player : Sandbox.Player
	{
		public Outfit Outfit { get; set;}
		CitizenAnimator npcAnim;

		[Net] 
		public TimeSince TimeSinceDied { get; set; }

		public Player()
		{
			Transmit = TransmitType.Always;
			npcAnim = new(this);
		}

		public override void Respawn()
		{
			SetModel("models/citizen/citizen.vmdl");
			SkillRecharge = 0;

			Animator = new StandardPlayerAnimator();
			Controller = new Controller();
			Camera = new Camera();

			Inventory?.DeleteContents();
			Inventory = new Inventory(this);
			
			EnableDrawing = true;
			EnableAllCollisions = true;
			EnableShadowInFirstPerson = true;
			EnableHideInFirstPerson = true;
			
			base.Respawn();

			Outfit?.Clear();
			Outfit = new Outfit(this);
			Outfit.ApplyOutfit();

			Game.CurrentRound?.OnPlayerRespawn(this);
			Team?.OnPlayerRespawned(this);
			Skill = BaseSkill.RandomSkill();
		}

		public override void CreateHull()
		{
			CollisionGroup = CollisionGroup.Player;
			AddCollisionLayer(CollisionLayer.Player);
			SetupPhysicsFromCapsule(PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius(72, 8));

			MoveType = MoveType.MOVETYPE_WALK;
			EnableHitboxes = true;
		}

		protected override void UseFail() 
		{ 
		
		}

		public override void OnAnimEventFootstep(Vector3 pos, int foot, float volume)
		{
			if (Host.IsServer) return;

			if (IsLocalPawn)
			{
				base.OnAnimEventFootstep(pos, foot, 0.5f);
				return;
			}

			float stepDistance = Vector3.DistanceBetween(Input.Position, pos);
			float stepVolume = (float)Math.Pow(1 - (stepDistance / 512), .5);

			if (stepVolume < 0)
				return;

			base.OnAnimEventFootstep(pos, foot, stepVolume);
		}

		DamageInfo lastDamage;
		public override void OnKilled()
		{
			if(Skill is not null && Skill.IsActive) Skill?.OnEnd();
			Skill = null;

			base.OnKilled();

			TimeSinceDied = 0;
			Controller = null;
			EnableDrawing = false;
			EnableAllCollisions = false;

			LifeState = LifeState.Dead;
			new Ragdoll(this, lastDamage);
			Camera = new SpectateCamera();

			Inventory.DeleteContents();
			Game.CurrentRound?.OnPlayerDied(this);
		}

		public override void TakeDamage(DamageInfo info)
		{
			lastDamage = info;
			base.TakeDamage(info);
		}

		public override void Simulate(Client cl)
		{
			GetActiveController()?.Simulate(cl, this, Animator);
			base.TickPlayerUse();
			this.SkillTick();
			
			if (ActiveChild is not null)
				SimulateActiveChild(cl, ActiveChild);

			if (LifeState == LifeState.Alive)
				if(Animator is null && npcAnim is not null)
					npcAnim.Tick(Controller.WishVelocity, Velocity, Controller.HasTag("ducked"));
		}
	}
}
