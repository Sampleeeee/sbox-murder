using Murder.UserInterface;
using Sandbox;
using System;
using System.Linq;
using Murder.Entities;

namespace Murder.Rounds
{
	public class Intermission : BaseRound
	{
		public override int RoundDuration => 10;
		public override string RoundName => "Intermission";

		protected override void OnStart()
		{
			Game.RespawnEnabled = true;
			
			foreach ( var client in Client.All )
			{
				if ( client.Pawn is not MurderPlayer player ) continue;

				player.Dead = false;

				player.Tags.Remove( "murderer" );
				player.Tags.Remove( "detective" );

				player.Ragdoll?.Delete();
				player.Ragdoll = null;

				player.Respawn();
			}

			foreach ( var gun in Entity.All.OfType<DroppedGun>() )
				gun.Delete();

			foreach ( var knife in Entity.All.OfType<ThrownKnife>() )
				knife.Delete();

			base.OnStart();
		}

		protected override void OnTimeUp()
		{
			Game.ChangeRound( new Preparing() );
			base.OnTimeUp();
		}
	}
}
