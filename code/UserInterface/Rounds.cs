using Murder.Rounds;
using Sandbox;
using Sandbox.UI;

namespace Murder.UserInterface
{
	[UseTemplate( "/UserInterface/Rounds.htm" )]
	public class Rounds : Panel
	{
		public string Status
		{
			get
			{
				var game = Game.Current as MurderGame;

				switch ( game?.Round )
				{
					case null:
						return "Waiting for Players";
					case Playing:
					{
						var player = Local.Pawn as MurderPlayer;

						if ( player.Tags.Has( "murderer" ) )
							return "Murderer";
				
						return player.Tags.Has( "detective" ) ? "Detective" : "Bystander";
					}
					default:
						return (Game.Current as MurderGame)?.Round?.RoundName ?? "Waiting for Players";
				}
			}
		}

		public string TimeLeft =>
			(Game.Current as MurderGame)?.Round?.TimeLeftFormatted ?? "";
	}
}
