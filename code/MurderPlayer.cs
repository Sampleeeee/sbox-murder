using System;
using Murder.Rounds;
using Murder.Weapons;
using Sandbox;

namespace Murder
{
	public partial class MurderPlayer : Player
	{
		[Net] public bool Dead { get; set; }
		[Net, Local] public ModelEntity Ragdoll { get; set; }
		
		[Net] public string FakeName { get; set; }
		[Net] public Color32 FakeColor { get; set; }

		private Sound? _taunt = null;
		
		public string SteamName { get; set; }

		private static string[] _taunts =
		{
			"taunt.help1", "taunt.scream1", "taunt.scream2", "taunt.scream3", "taunt.scream4", "taunt.scream5",
			"taunt.morose1", "taunt.morose2", "taunt.morose3", "taunt.morose4", "taunt.morose5", "taunt.funny1",
			"taunt.funny2", "taunt.funny3", "taunt.funny4", "taunt.funny5", "taunt.funny6", "taunt.funny7"
		};
		
		public MurderPlayer()
		{
			Inventory = new BaseInventory( this );
			Camera = new FirstPersonCamera();
		}

		public override void Respawn()
		{
			if ( !(Game.Current as MurderGame).RespawnEnabled )
				return;
			
			base.Respawn();
			
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new MurderWalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			
			Inventory.DeleteContents();
			Inventory.Add( new Fists(), true );

			Dress();
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( Input.ActiveChild is not null )
				ActiveChild = Input.ActiveChild;

			if ( LifeState != LifeState.Alive )
				return;

			SimulateActiveChild( cl, ActiveChild );
			TauntSimulate();
		}

		// TODO This could be much better, with a menu or something
		// For now this is fine, maybe add multiple controls to play the different kinds of taunts
		private void TauntSimulate()
		{
			if ( Game.Current is not MurderGame game ) return;
			if ( game.Round is Playing && Dead ) return;
			
			_taunt?.SetPosition( EyePos );
			
			if ( !IsServer && !Input.Pressed( InputButton.Flashlight ) ) return;
			
			_taunt?.Stop();
			_taunt = Sound.FromEntity( _taunts.Random(), this );
		}

		public void MakeSpectator()
		{
			EnableAllCollisions = false;
			EnableDrawing = false;
			Controller = null;
			Camera = new SpectateCamera();
		}

		public override void OnKilled()
		{
			base.OnKilled();
			Inventory.DeleteContents();

			if ( GetModelName() is not null )
			{
				var ragdoll = new ModelEntity();
				ragdoll.SetModel( GetModelName() );
				ragdoll.Position = Position;
				ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

				ragdoll.CopyBonesFrom( this );
				ragdoll.SetRagdollVelocityFrom( this );

				foreach ( var child in Children )
				{
					if ( child is not ModelEntity entity ) continue;

					string model = entity.GetModelName();
					if ( model is null || !model.Contains( "clothes" ) ) continue;

					var clothing = new ModelEntity();
					clothing.SetModel( model );
					clothing.SetParent( ragdoll, true );
					clothing.RenderColor = entity.RenderColor;
				}

				Ragdoll = ragdoll;
			}

			Strip();
			EnableAllCollisions = false;
			EnableDrawing = false;

			Controller = null;
			Camera = new SpectateCamera();

			Dead = true;
		}
	}
}
