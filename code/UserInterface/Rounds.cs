using Sandbox;
using Sandbox.UI;

namespace Murder.UserInterface
{
	[UseTemplate( "/UserInterface/Rounds.htm" )]
	public class Rounds : Panel
	{
		public string Status =>
			(Game.Current as MurderGame)?.Round?.RoundName ?? "Waiting for Players";
	}
}
