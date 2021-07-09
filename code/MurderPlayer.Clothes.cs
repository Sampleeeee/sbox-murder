using System.Threading.Tasks;
using Murder.Rounds;
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

			EnableShadowInFirstPerson = true;
			EnableHideInFirstPerson = true;
		}

	}
	
	public partial class MurderPlayer
	{
		public Clothing Coat { get; set; }
		private Clothing _shirt;
		private Clothing _pants;
		private Clothing _shoes;
		private Clothing _hair;
		
		public void Dress()
		{
			Strip();

			Coat = new Clothing( this, "models/citizen_clothes/jacket/labcoat.vmdl", true );
			// _shirt = new Clothing( this, "models/citizen_clothes/shirt/shirt_longsleeve.scientist.vmdl" );
			// _pants = new Clothing( this, "models/citizen_clothes/trousers/trousers.lab.vmdl" );
			_shoes = new Clothing( this, "models/citizen_clothes/shoes/shoes.workboots.vmdl" );
			_hair = new Clothing( this, "models/citizen_clothes/hair/hair_malestyle02.vmdl" );
		}

		public void Strip()
		{
			Coat?.Delete();
			_shirt?.Delete();
			_pants?.Delete();
			_shoes?.Delete();
			_hair?.Delete();
		}
	}
}
