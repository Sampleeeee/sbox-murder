using Murder.UserInterface;

namespace Murder.Rounds
{
	public class Ending : BaseRound
	{
		public override int RoundDuration => 5;
		public override string RoundName => "Ending";

		public override void OnPlayerSpawn( MurderPlayer player )
		{
			player.MakeSpectator();
			base.OnPlayerSpawn( player );
		}

		protected override void OnStart()
		{
			Game.RespawnEnabled = false;

			base.OnStart();
		}

		protected override void OnTimeUp()
		{
			BlackScreen.HideScreen();
			
			Game.ChangeRound( new Intermission() );
			base.OnTimeUp();
		}
	}
}
