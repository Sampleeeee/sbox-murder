using System.Threading.Tasks;
using Murder.Rounds;
using Murder.UserInterface;
using Sandbox;

namespace Murder
{
	[Library( "murder", Title = "Murder" )]
	public partial class MurderGame : Game
	{
		[Net] public BaseRound Round { get; set; }
		[Net] public bool RespawnEnabled { get; set; } = true;
		
		public MurderPlayer LastMurderer { get; set; }
		public MurderPlayer LastDetective { get; set; }
		
		public MurderGame()
		{
			if ( IsServer )
				_ = new MurderHudEntity();

			_ = StartTickTimer();
		}

		public async Task StartTickTimer()
		{
			while ( true )
			{
				await Task.NextPhysicsFrame();
				OnTick();
			}
		}

		public async Task StartSecondTimer()
		{
			while ( true )
			{
				await Task.DelaySeconds( 1 );
				OnSecond();
			}
		}

		public override void PostLevelLoaded()
		{
			base.PostLevelLoaded();
			_ = StartSecondTimer();
		}

		private void OnSecond()
		{
			CheckMinimumPlayers();
			Round?.OnSecond();
		}

		private void OnTick()
		{
			Round?.OnTick();
		}

		public void ChangeRound( BaseRound round )
		{
			Assert.NotNull( round );

			Round?.Finish();
			Round = round;
			Round?.Start();
		}

		public override void OnKilled( Entity entity )
		{
			if ( entity is MurderPlayer player )
				Round?.OnPlayerKilled( player );
			
			base.OnKilled( entity );
		}

		public void CheckMinimumPlayers()
		{
			if ( Client.All.Count < 2 ) return;

			if ( Round is null )
				ChangeRound( new Intermission() );
		}

		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );

			var player = new MurderPlayer();
			cl.Pawn = player;
			player.SteamName = cl.Name;

			player.Respawn();
		}
	}
}
