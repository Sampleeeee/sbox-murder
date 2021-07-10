using System.Net.Http.Headers;
using System.Threading.Tasks;
using Murder.Entities;
using Sandbox;

namespace Murder.Weapons
{
	// Thanks Argonium for helping he setup knife model / animations
	[Library( "weapon_knife", Title = "Knife" )]
	public class Knife : MurderWeapon
	{
		public override int Bucket => 2;
		public int Distance => 80;

		public override string ViewModelPath => "weapons/rust_boneknife/v_rust_boneknife.vmdl";

		public override void Spawn()
		{
			base.Spawn();
			
			SetModel( "weapons/rust_boneknife/rust_boneknife.vmdl" );
		}

		public override void SimulateAnimator( PawnAnimator anim )
		{
			base.SimulateAnimator( anim );

			anim.SetParam( "holdtype", 4 );
		}
		
		public override bool CanPrimaryAttack() =>
			base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
		
		private void MeleeStrike( float damage, float force )
		{
			var forward = Owner.EyeRot.Forward;
			forward = forward.Normal;

			foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * Distance, 3f ) )
			{
				if ( !tr.Entity.IsValid() )
					continue;

				tr.Surface.DoBulletImpact( tr );

				if ( !IsServer )
					continue;

				using ( Prediction.Off() )
				{
					var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100 * force, damage )
						.UsingTraceResult( tr )
						.WithAttacker( Owner )
						.WithWeapon( this );

					tr.Entity.TakeDamage( damageInfo );
				}
			}
		}

		public override void AttackPrimary()
		{
			TimeSincePrimaryAttack = 0;

			PlaySound( "rust_boneknife.attack" );
			ViewModelEntity?.SetAnimBool( "fire", true );
			SetAnimBool( "fire", true );
			MeleeStrike( 150f, 1.5f );
			
			base.AttackPrimary();
		}

		public override void AttackSecondary()
		{
			TimeSinceSecondaryAttack = 0;

			if ( !IsServer ) return;
			
			var knife = new ThrownKnife();
			knife.Position = Owner.EyePos + Owner.EyeRot.Forward * 40;
			knife.Rotation = Owner.EyeRot;
			knife.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
			knife.PhysicsGroup.Velocity = Owner.EyeRot.Forward * 2000;

			var owner = Owner;
			
			Owner.Inventory.DeleteContents();
			Owner.Inventory.Add( new Fists() );

			_ = GivePlayerKnife( owner, knife );
		}

		private async Task GivePlayerKnife( Entity owner, Entity knife )
		{
			await Task.DelaySeconds( 10 );

			if ( !knife.IsValid() ) return;
			
			owner?.Inventory.Add( new Knife() );
			knife.Delete();
		}
	}
}
