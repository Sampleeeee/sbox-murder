using System;
using System.Linq;
using Murder.Weapons;
using Sandbox;

namespace Murder.Entities
{
	[Library( "ent_thrown_knife", Title = "Dropped Gun" )]
	public class ThrownKnife : Prop
	{
		public override void Spawn()
		{
			SetModel( "weapons/rust_boneknife/rust_boneknife.vmdl" );
			GlowColor = Color.Red;

			base.Spawn();
		}

		[Event.Tick]
		private void OnTick()
		{
			if ( !IsServer ) return;

			foreach ( var player in All.OfType<MurderPlayer>()
				.Where( x => x.Tags.Has( "murderer" ) && x.Position.Distance( Position ) <= 50f ) )
			{
				player.Inventory.Add( new Knife(), true );
				Delete();

				break;
			}
		}
		
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			GlowActive = true;
			GlowState = GlowStates.GlowStateOn;
			GlowDistanceStart = -32;
			GlowDistanceEnd = 4096;
		}

		public override void StartTouch( Entity other )
		{
			if ( other is not MurderPlayer player ) return;
			if ( player.Dead ) return;

			Log.Info( Velocity.Length );
			if ( Velocity.Length >= 100f )
				player.TakeDamage( new DamageInfo {Damage = 100}.WithAttacker( Owner ).WithWeapon( this ) );
			
			base.StartTouch( other );
		}
	}
}
