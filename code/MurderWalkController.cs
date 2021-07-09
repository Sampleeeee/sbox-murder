using Murder.Rounds;
using Sandbox;

namespace Murder
{
	public class MurderWalkController : WalkController
	{
		public override float GetWishSpeed()
		{
			float ws = Duck.GetWishSpeed();
			if ( ws >= 0 ) return ws;

			if ( (Game.Current as MurderGame)?.Round is Preparing )
				return 0f;

			if ( Input.Down( InputButton.Run ) ) return Pawn.Tags.Has( "murderer" ) ? DefaultSpeed + 50f : DefaultSpeed;
			return Input.Down( InputButton.Walk ) ? WalkSpeed : DefaultSpeed;
		}
	}
}
