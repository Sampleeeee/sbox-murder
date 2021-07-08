using Sandbox;

namespace Murder.Weapons
{
	[Library( "weapon_revolver", Title = "Pistol" )]
	public class Revolver : MurderWeapon
	{
		public override float PrimaryRate => 0.75f;

		public override int Bucket => 2;

		public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

		public override void Spawn()
		{
			base.Spawn();
			
			SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
		}

		public override bool CanPrimaryAttack() =>
			base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
		
		public override void AttackPrimary()
		{
			Log.Info( TimeSincePrimaryAttack );
			
			base.AttackPrimary();

			TimeSincePrimaryAttack = 0;
			TimeSinceSecondaryAttack = 0;
			
			PlaySound( "rust_pistol.shoot" );

			ShootEffects();
			ShootBullet( 0.05f, 1.5f, 100f, 3f );
		}
	}
}
