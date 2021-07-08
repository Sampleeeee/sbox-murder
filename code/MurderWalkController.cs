using Sandbox;

namespace Murder
{
	public class MurderWalkController : WalkController
	{
		public override float GetWishSpeed()
		{
			float ws = Duck.GetWishSpeed();
			if ( ws >= 0 ) return ws;

			if ( Input.Down( InputButton.Run ) ) return Pawn.Tags.Has( "murderer" ) ? SprintSpeed : DefaultSpeed;
			return Input.Down( InputButton.Walk ) ? WalkSpeed : DefaultSpeed;
		}
	}
}
