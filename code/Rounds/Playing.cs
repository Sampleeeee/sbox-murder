using System;
using Murder.Entities;
using Murder.UserInterface;
using Murder.Weapons;
using Sandbox;

namespace Murder.Rounds
{
	public class Playing : BaseRound
	{
		private static string[] _fakeNames =
		{
			"Adam", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "Ida", "Juliet", "Kilo",
			"Lincoln", "Mike", "Nora", "Ocean", "Paul", "Queen", "Robert", "Sam", "Union", "Victor", "Whiskey",
			"Xray", "Zulu"
		};

		public override int RoundDuration => 240;
		public override string RoundName => "Playing";
		
		public override void OnPlayerKilled( MurderPlayer player )
		{
			if ( Players.Contains( player ) )
				Players.Remove( player );
			
			player.MakeSpectator();
			player.Dead = true;
			
			var attacker = player.LastAttacker;

			if ( attacker is not null )
			{

				if ( attacker.Tags.Has( "detective" ) && !player.Tags.Has( "murderer" ) )
				{
					attacker.TakeDamage( new DamageInfo { Damage = 10000 } );
				}
			}

			if ( player.Tags.Has( "detective" ) )
			{
				var gun = new DroppedGun();
				gun.Position = player.EyePos + player.EyeRot.Forward * 40;
				gun.Rotation = Rotation.LookAt( Vector3.Random.Normal );
				gun.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
				gun.PhysicsGroup.Velocity = player.EyeRot.Forward * 500;
			}

			var alivePlayers = 0;
			foreach ( var client in Client.All )
			{
				if ( client.Pawn is not MurderPlayer pl ) continue;

				if ( !pl.Dead )
					alivePlayers++;
			}

			if ( player.Tags.Has( "murderer" ) )
			{
				BlackScreen.ShowTitle( "Bystanders win!", Color.Blue );
				BlackScreen.ShowDescription( $"{player.FakeName} ({player.GetClientOwner().Name}) was the murderer!", Color.White );
				
				// TODO This may cause issues if people leave and shit
				Game.ChangeRound( new Ending() );
			}
			else if ( alivePlayers <= 1 )
			{
				BlackScreen.ShowTitle( "Murderers win!", Color.Red );
				BlackScreen.ShowDescription( $"{Game.Murderer.FakeName} ({Game.Murderer.GetClientOwner().Name}) was the murderer!",
					Color.White );
				
				// TODO This may cause issues if people leave and shit
				Game.ChangeRound( new Ending() );
			}
		}

		public override void OnPlayerSpawn( MurderPlayer player )
		{
			player.MakeSpectator();

			if ( Players.Contains( player ) )
				Players.Remove( player );
			
			base.OnPlayerSpawn( player );
		}

		protected override void OnStart()
		{
			foreach ( var client in Client.All )
			{
				if ( client.Pawn is not MurderPlayer player ) continue;

				player.Ragdoll?.Delete();
				player.Ragdoll = null;
				
				player.FakeColor = Color.Random.ToColor32();
				player.FakeName = _fakeNames.Random();
				
				player.Respawn();
				player.Coat.RenderColor = player.FakeColor;
			}

			Game.Murderer.Inventory.Add( new Knife() );
			Game.Detective.Inventory.Add( new Revolver() );
			
			BlackScreen.HideScreen();
			base.OnStart();
		}

		protected override void OnFinish()
		{
			Game.RespawnEnabled = true;
			
			foreach ( var client in Client.All )
			{
				if ( client.Pawn is not MurderPlayer player ) continue;
				
				player.Tags.Remove( "murderer" );
				player.Tags.Remove( "detective" );

				// player.FakeName = client.Name;
				player.FakeColor = Color32.White;
				
				player.Respawn();
			}
			
			base.OnFinish();
		}
	}
}
