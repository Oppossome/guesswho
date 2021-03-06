using guesswho.animation;
using guesswho.weapons;
using guesswho.skills;
using guesswho.ui;
using guesswho.teams;
using guesswho.player;
using Sandbox;
using System;
using System.Collections.Generic;

namespace guesswho
{
	public partial class Player : Sandbox.Player, IHumanoid
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
			Skill = BaseSkill.RandomSkill();
			Team?.OnPlayerRespawned(this);
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

		public override void OnAnimEventFootstep(Vector3 Position, int foot, float volume)
		{
			if (Host.IsServer) return;

			if (IsLocalPawn)
			{
				base.OnAnimEventFootstep(Position, foot, 0.5f);
				return;
			}

			float stepDistance = Vector3.DistanceBetween(Input.Position, Position);
			float stepVolume = (float)Math.Pow(1 - (stepDistance / 512), .5);

			if (stepVolume < 0)
				return;

			base.OnAnimEventFootstep(Position, foot, stepVolume);
		}

		DamageInfo lastDamage;
		public override void OnKilled()
		{
			if(Skill is not null && Skill.IsActive) Skill?.Reset();
			Skill = null;

			if (Team is Hunters)
				if(lastDamage.Attacker is Player ply && ply.LifeState == LifeState.Alive)
					ply.Health = Math.Min(100, ply.Health + 20);

			KillLog.AddEntry(lastDamage, this);
			base.OnKilled();
			Outfit.Clear();

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

		public string GetName()
		{
			return this.Client.Name;
		}
	}
}
