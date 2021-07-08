using System;
using System.Linq;
using Murder.UserInterface;
using Sandbox;

namespace Murder.Rounds
{
	public class Preparing : BaseRound
	{
		public override int RoundDuration => 5;
		public override string RoundName => "Preparing";

		public override void OnPlayerSpawn( MurderPlayer player )
		{
			if ( !Players.Contains( player ) )
				Players.Add( player );
			
			base.OnPlayerSpawn( player );
		}

		private void FindNewDetective()
		{
			var pawn = Client.All[new Random().Next( Client.All.Count )].Pawn;

			pawn.Tags.Add( "detective" );
			Game.LastDetective = pawn as MurderPlayer;

			BlackScreen.ShowTitle( To.Single( pawn.GetClientOwner() ), "You are a detective!", Color.Blue );
			BlackScreen.ShowDescription( To.Single( pawn.GetClientOwner() ),
				"You have been given a revolver. Find the murderer and kill them.", Color.White );
		}

		private void FindNewMurderer()
		{
			var pawn = Client.All[new Random().Next( Client.All.Count )].Pawn;

			if ( Game.LastDetective != pawn )
			{
				pawn.Tags.Add( "murderer" );
				Game.LastMurderer = pawn as MurderPlayer;

				BlackScreen.ShowTitle( To.Single( pawn.GetClientOwner() ), "You are a murderer!", Color.Red );
				BlackScreen.ShowDescription( To.Single( pawn.GetClientOwner() ),
					"You have one job. Kill everyone.", Color.White );

				return;
			}

			FindNewMurderer();
		}

		protected override void OnStart()
		{
			Game.RespawnEnabled = false;

			FindNewDetective();
			FindNewMurderer();

			var to = To.Multiple( Client.All.Where( x =>
				!x.Pawn.Tags.Has( "murderer" ) && !x.Pawn.Tags.Has( "detective" ) ) );
			
			BlackScreen.ShowTitle( to, "You are a bystander", Color.White );
			BlackScreen.ShowDescription( to, "There is a murderer on the loose. Do not die.", Color.White );
		}

		protected override void OnTimeUp()
		{
			Game.ChangeRound( new Playing() );
			base.OnTimeUp();
		}
	}
}
