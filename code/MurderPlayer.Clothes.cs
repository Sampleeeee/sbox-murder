using Sandbox;

namespace Murder
{
	public class Clothing : ModelEntity
	{
		public Clothing() : base() { }
		
		public Clothing( MurderPlayer player, string model, bool addColor = false )
		{
			SetModel( model );
			SetParent( player, true );

			RenderColor = player.FakeColor;
			
			EnableShadowInFirstPerson = true;
			EnableHideInFirstPerson = true;
		}
	}
	
	public partial class MurderPlayer
	{
		private Clothing _coat;
		private Clothing _shirt;
		private Clothing _pants;
		private Clothing _shoes;
		private Clothing _hair;
		
		public void Dress()
		{
			Log.Info( "Dresseing" );
			
			Strip();

			_coat = new Clothing( this, "models/citizen_clothes/jacket/labcoat.vmdl", true );
			// _shirt = new Clothing( this, "models/citizen_clothes/shirt/shirt_longsleeve.scientist.vmdl" );
			// _pants = new Clothing( this, "models/citizen_clothes/trousers/trousers.lab.vmdl" );
			_shoes = new Clothing( this, "models/citizen_clothes/shoes/shoes.workboots.vmdl" );
			_hair = new Clothing( this, "models/citizen_clothes/hair/hair_malestyle02.vmdl" );
		}

		public void Strip()
		{
			_coat?.Delete();
			_shirt?.Delete();
			_pants?.Delete();
			_shoes?.Delete();
			_hair?.Delete();
		}
	}
}
