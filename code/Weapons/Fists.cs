using Sandbox;

namespace Murder.Weapons
{
	[Library( "weapon_fists", Title = "Fists" )]
	public class Fists : MurderWeapon
	{
		public override void SimulateAnimator( PawnAnimator anim )
		{
			base.SimulateAnimator( anim );
			anim.SetParam( "holdtype", 0 );
		}
	}
}
