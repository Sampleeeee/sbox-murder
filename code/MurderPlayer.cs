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
		
		public string SteamName { get; set; }
		
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
			base.Respawn();
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( Input.ActiveChild is not null )
				ActiveChild = Input.ActiveChild;

			if ( LifeState != LifeState.Alive )
				return;

			SimulateActiveChild( cl, ActiveChild );
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
