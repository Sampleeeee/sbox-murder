using System;
using System.Linq;
using Murder.Weapons;
using Sandbox;

namespace Murder.Entities
{
	[Library("ent_dropped_gun", Title = "Dropped Gun")]
	public class DroppedGun : Prop
	{
		public override void Spawn()
		{
			SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );

			base.Spawn();
		}

		[Event.Tick]
		private void OnTick()
		{
			if ( !IsServer ) return;
			
			foreach ( var player in All.OfType<MurderPlayer>()
				.Where( x => !x.Tags.Has( "murderer" ) && x.Position.Distance( Position ) <= 50f ) )
			{
				player.Inventory.Add( new Revolver(), true );
				Delete();
				
				break;
			}
		}
	}
}
