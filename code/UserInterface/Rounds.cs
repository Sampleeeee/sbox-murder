using Murder.Rounds;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Murder.UserInterface
{
	[UseTemplate( "/UserInterface/Rounds.htm" )]
	public class Rounds : Panel
	{
		public Label Name { get; set; }

		public string TimeLeft =>
			(Game.Current as MurderGame)?.Round?.TimeLeftFormatted ?? "";
		
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

		public override void Tick()
		{
			if ( Game.Current is not MurderGame game ) return;
			if ( Local.Pawn is not MurderPlayer player ) return;

			if ( game.Round is Playing )
			{
				Name ??= Add.Label( $"{player.FakeName}" );
				Name.Style.FontColor = player.FakeColor.ToColor();
			}
			else
			{
				Name?.Delete();
				Name = null;
			}

			base.Tick();
		}
	}
}
